namespace Core.Instrumentation.Performance
{
	using System.Diagnostics;
	using PostSharp.Aspects;

	public sealed class InstrumentInvocationsPerSecondAttribute : PerformanceCounterAttribute
	{
		public InstrumentInvocationsPerSecondAttribute(string categoryName, string counterName)
			: base(categoryName, counterName, PerformanceCounterType.RateOfCountsPerSecond32)
		{
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
			if (this.PerformanceCounter != null)
			{
				this.PerformanceCounter.Increment();
			}
		}
	}
}
