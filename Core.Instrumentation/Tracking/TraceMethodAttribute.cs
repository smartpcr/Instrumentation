namespace Core.Instrumentation.Tracking
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using Core.Instrumentation.ETW;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	[Serializable]
	public sealed class TraceMethodAttribute : OnMethodBoundaryAspect
	{
		#region props
		public Categories Category { get; private set; }
		public Layers Layer { get; private set; }

		[NonSerialized]
		private string enteringMessage;
		[NonSerialized]
		private string exitingMessage;
		[NonSerialized]
		private string methodName;
		#endregion

		#region ctor
		public TraceMethodAttribute():this(Categories.Default)
		{
		}

		public TraceMethodAttribute(Categories category):this(category, Layers.Unknown)
		{
		}

		public TraceMethodAttribute(Categories category, Layers layer)
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
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
			TraceEventSource.Log.BeforeMethod(this.enteringMessage, this.Category, this.Layer);
			args.MethodExecutionTag = Utility.GlobalStopwatch.ElapsedTicks;
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			decimal milliseconds = Utility.TicksDiffInMs((long)args.MethodExecutionTag);
			TraceEventSource.Log.TimeMethod(milliseconds, this.Category, this.Layer);
			TraceEventSource.Log.AfterMethod(this.exitingMessage, this.Category, this.Layer);
		}

		#endregion
	}
}
