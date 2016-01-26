namespace Core.Instrumentation.Tracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using Core.Instrumentation.Tracings;
    using Newtonsoft.Json;
    using PostSharp.Aspects;
    using PostSharp.Extensibility;


    public enum CallFlowType
    {
        Layer,
        Method,
        Class,
        Assembly
    }

    [Serializable]
    public class AsyncCallContext
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public Layers Layer { get; set; }
        public string InArgs { get; set; }
    }

    [MulticastAttributeUsage(MulticastTargets.InstanceConstructor | MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
    public class TraceCallGraphAspect : OnMethodBoundaryAspect
    {
        #region props
        public Categories Category { get; private set; }
        public Layers Layer { get; private set; }
        public CallFlowType FlowType { get; private set; }
        public string CorrelationId { get; private set; }
        public bool LogCallStack { get; private set; }
        
        [NonSerialized]
        private string enteringMessage;
        [NonSerialized]
        private string exitingMessage;
        [NonSerialized]
        private string methodName;
        #endregion

        public TraceCallGraphAspect(Categories category, Layers layer, CallFlowType flowType=CallFlowType.Layer, bool logCallStack=false)
        {
            this.Category = category;
            this.Layer = layer;
            this.FlowType = flowType;
            this.LogCallStack = logCallStack;
        }

        #region overrides
        public override void RuntimeInitialize(MethodBase method)
        {
            this.methodName =
                method.DeclaringType == null
                ? method.Name
                : method.DeclaringType.FullName + "." + method.Name;
            this.enteringMessage = "Entering " + methodName;
            this.exitingMessage = "Exiting " + methodName;
            switch (this.FlowType)
            {
                case CallFlowType.Layer:
                    this.CorrelationId = this.Layer.ToString();
                    break;
                case CallFlowType.Assembly:
                    this.CorrelationId = this.GetType().Assembly.FullName;
                    break;
                case CallFlowType.Class:
                    this.CorrelationId = this.GetType().FullName;
                    break;
                default:
                    this.CorrelationId = this.GetType().FullName + "." + methodName;
                    break;
            }
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            string argValues = string.Empty;
            if (args.Arguments != null && args.Arguments.Count > 0)
            {
                argValues = JsonConvert.SerializeObject(args.Arguments);
            }
            Bootstrap.GetLogger().Enter(this.Category, this.Layer, this.GetType().FullName, this.methodName, this.enteringMessage, argValues);

            // create new call context
            var callContext = new AsyncCallContext()
            {
                ClassName = this.GetType().FullName,
                MethodName = this.methodName,
                Layer = this.Layer,
                InArgs = argValues
            };
            var callStack = CallContext.LogicalGetData(this.CorrelationId) as Stack<AsyncCallContext>;
            if (callStack == null)
            {
                callStack = new Stack<AsyncCallContext>();
                CallContext.LogicalSetData(this.CorrelationId, callStack);
            }
            callStack.Push(callContext);
            if (this.LogCallStack)
            {
                Bootstrap.GetLogger().Log(callStack);
            }

            args.MethodExecutionTag = Utility.GlobalStopwatch.ElapsedTicks;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            string argValue = string.Empty;
            if (args.ReturnValue != null)
            {
                argValue = JsonConvert.SerializeObject(args.ReturnValue);
            }
            long milliseconds = (long)Utility.TicksDiffInMs((long)args.MethodExecutionTag);
            Bootstrap.GetLogger().Exit(this.Category, this.Layer, this.GetType().FullName, this.methodName, this.exitingMessage, argValue, milliseconds);

            // popup current call context

            var callStack = CallContext.LogicalGetData(this.CorrelationId) as Stack<AsyncCallContext>;
            if (callStack != null && callStack.Count > 0)
            {
                callStack.Pop();
                if (this.LogCallStack)
                {
                    Bootstrap.GetLogger().Log(callStack);
                }
                if (callStack.Count == 0)
                {
                    callStack = null;
                }
            }
            
        }

        public override void OnException(MethodExecutionArgs args)
        {
            // handle exception

            // clear call context
            CallContext.LogicalSetData(this.CorrelationId, null);
        }

        #endregion
    }
}
