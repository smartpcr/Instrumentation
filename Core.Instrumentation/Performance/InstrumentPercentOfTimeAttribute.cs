namespace Core.Instrumentation.Performance
{
	using System;
	using System.Diagnostics;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[MulticastAttributeUsage(
		MulticastTargets.Method, 
		TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
	[AttributeUsage(
		AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, 
		AllowMultiple = true)]
	[Serializable]
	public sealed class InstrumentPercentOfTimeAttribute : PerformanceCounterAttribute
	{
		public InstrumentPercentOfTimeAttribute(string categoryName, string counterName)
			: base(categoryName, counterName, PerformanceCounterType.CounterTimer)
		{
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
			args.MethodExecutionTag = Utility.GlobalStopwatch.ElapsedTicks;
			base.OnEntry(args);
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			if (this.PerformanceCounter != null)
			{
				long ticks = Utility.TicksDiff((long) args.MethodExecutionTag);
				this.PerformanceCounter.IncrementBy(ticks);
			}
		}
	}
}
