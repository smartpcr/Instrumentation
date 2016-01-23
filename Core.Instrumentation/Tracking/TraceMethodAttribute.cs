namespace Core.Instrumentation.Tracking
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	[Serializable]
	public sealed class TraceMethodAttribute : OnMethodBoundaryAspect
	{
		#region props
		public string Category { get; private set; }
		[NonSerialized]
		private string enteringMessage;
		[NonSerialized]
		private string exitingMessage;
		[NonSerialized]
		private string methodName;
		#endregion

		#region ctor
		public TraceMethodAttribute()
		{
		}

		public TraceMethodAttribute(string categoryName)
		{
			this.Category = categoryName;
		}
		#endregion

		#region overrides

		public override void RuntimeInitialize(MethodBase method)
		{
			this.methodName = method.DeclaringType.FullName + "." + method.Name;
			this.enteringMessage = "Entering " + methodName;
			this.exitingMessage = "Exiting " + methodName;
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
			Trace.Indent();
			Trace.WriteLine(this.enteringMessage, this.Category);
			args.MethodExecutionTag = Utility.GlobalStopwatch.ElapsedTicks;
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			decimal milliseconds = Utility.TicksDiffInMs((long)args.MethodExecutionTag);

			Trace.Indent();
			Trace.WriteLine(string.Format("{0}ms elapsed in {1}", milliseconds, this.methodName), this.Category);
			Trace.Unindent();

			Trace.WriteLine(this.exitingMessage, this.Category);
			Trace.Unindent();
		}

		#endregion
	}
}
