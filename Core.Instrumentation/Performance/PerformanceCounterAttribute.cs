namespace Core.Instrumentation.Performance
{
    using System;
    using System.Diagnostics;
    using PostSharp.Aspects;
    using PostSharp.Serialization;

    /// <summary>
    /// base class for all perf counter aspects
    /// </summary>
    /// <remarks>this class takes of initialization and indentification</remarks>
    [PSerializable]
    public class PerformanceCounterAttribute : OnMethodBoundaryAspect
    {
        #region members
        public string CategoryName { get; private set; }
        public string CounterName { get; private set; }
        public PerformanceCounterType CounterType { get; private set; }
        public PerformanceCounterType? BaseCounterType { get; private set; }
        [NonSerialized]
        private PerformanceCounter performanceCounter, basePerformanceCounter;
        #endregion
    }
}
