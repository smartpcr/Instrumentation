namespace Core.Instrumentation.Tracking
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using Core.Instrumentation.Tracings;
	using Core.IoC;
	using Newtonsoft.Json;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[MulticastAttributeUsage(MulticastTargets.InstanceConstructor | MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
	public sealed class TraceMethodAspect : OnMethodBoundaryAspect
	{
		#region props
		public Categories Category { get; private set; }
		public Layers Layer { get; private set; }

        [NonSerialized]
        private ITraceLogger logger;
		[NonSerialized]
		private string enteringMessage;
		[NonSerialized]
		private string exitingMessage;
		[NonSerialized]
		private string methodName;
		#endregion

		#region ctor
		public TraceMethodAspect():this(Categories.Default)
		{
		}

		public TraceMethodAspect(Categories category):this(category, Layers.Unknown)
		{
		}

		public TraceMethodAspect(Categories category, Layers layer)
		{
			this.Category = category;
			this.Layer = layer;
		}
		#endregion

		#region overrides

		public override void RuntimeInitialize(MethodBase method)
		{
			this.methodName = 
				method.DeclaringType==null
				? method.Name 
				: method.DeclaringType.FullName + "." + method.Name;
			this.enteringMessage = "Entering " + methodName;
			this.exitingMessage = "Exiting " + methodName;
		    if (logger == null)
		    {
		        logger = Bootstrap.GetLogger();
		    }
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
		    string argValues = string.Empty;
		    if (args.Arguments != null && args.Arguments.Count > 0)
		    {
		        argValues = JsonConvert.SerializeObject(args.Arguments);
		    }
            logger.Enter(this.Category, this.Layer, this.GetType().FullName, this.methodName, this.enteringMessage, argValues);
			args.MethodExecutionTag = Utility.GlobalStopwatch.ElapsedTicks;
		}

		public override void OnExit(MethodExecutionArgs args)
		{
		    string argValue = string.Empty;
		    if (args.ReturnValue != null)
		    {
		        argValue = JsonConvert.SerializeObject(args.ReturnValue);
		    }
		    long milliseconds = (long) Utility.TicksDiffInMs((long) args.MethodExecutionTag);
            //EtwTraceEventSource.Log.TimeMethod(milliseconds, this.Category, this.Layer);
            logger.Exit(this.Category, this.Layer, this.GetType().FullName, this.methodName, this.exitingMessage,argValue, milliseconds);
		}

		#endregion
	}
}
