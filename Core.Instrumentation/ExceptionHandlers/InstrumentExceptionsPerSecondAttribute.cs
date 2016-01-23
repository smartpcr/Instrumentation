namespace Core.Instrumentation.ExceptionHandlers
{
	using System.Diagnostics;
	using Core.Instrumentation.Performance;
	using PostSharp.Aspects;

	public sealed class InstrumentExceptionsPerSecondAttribute : PerformanceCounterAttribute
	{
		public InstrumentExceptionsPerSecondAttribute(string categoryName, string counterName)
			: base(categoryName, counterName, PerformanceCounterType.RateOfCountsPerSecond32)
		{
		}

		public override void OnException(MethodExecutionArgs args)
		{
			if (this.PerformanceCounter != null)
			{
				this.PerformanceCounter.Increment();
			}
		}
	}
}
