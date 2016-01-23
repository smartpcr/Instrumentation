namespace Core.Instrumentation.ExceptionHandlers
{
	using System;
	using System.Diagnostics;
	using Core.Instrumentation.Performance;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	[Serializable]
	public sealed class InstrumentExceptionRateAttribute : PerformanceCounterAttribute
	{
		public InstrumentExceptionRateAttribute(string categoryName, string counterName)
			: base(categoryName, counterName, PerformanceCounterType.AverageCount64, PerformanceCounterType.AverageBase)
		{
		}

		public override void OnException(MethodExecutionArgs args)
		{
			if (this.PerformanceCounter != null)
			{
				this.PerformanceCounter.Increment();
			}
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			if (this.PerformanceCounter != null)
			{
				this.BasePerformanceCounter.Increment();
			}
		}
	}
}
