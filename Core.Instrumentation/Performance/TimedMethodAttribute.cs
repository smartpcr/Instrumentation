namespace Core.Instrumentation.Performance
{
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using PostSharp.Aspects;
    using PostSharp.Serialization;

    [PSerializable]
    public class TimedMethodAttribute : OnMethodBoundaryAspect
    {
        #region fields & props

        private string eventPath;
        private string[] parameterNames;
        public bool IncludeArguments { get; private set; }
        #endregion

        #region ctor

        public TimedMethodAttribute(bool includeArguments = true)
        {
            this.IncludeArguments = includeArguments;
            // works in async mode
            this.ApplyToStateMachine = true;
        }
        #endregion

        #region overrides

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            this.eventPath = method.DeclaringType.FullName.Replace(".", "/").Replace("+", "/") + "/" + method.Name;
            if (this.IncludeArguments)
            {
                this.parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
            }
        }


        public override void OnEntry(MethodExecutionArgs args)
        {
            //IAnalyticsRequest request = ServerAnalytics.CurrentRequest;
            base.OnEntry(args);
        }

        #endregion
    }
}
