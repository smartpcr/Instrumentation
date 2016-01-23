namespace Core.Instrumentation.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using PostSharp.Aspects;

    /// <summary>
    /// base class for all perf counter aspects
    /// </summary>
    /// <remarks>this class takes of initialization and indentification</remarks>
    [Serializable]
    public abstract class PerformanceCounterAttribute : OnMethodBoundaryAspect
    {
        #region members
        public string CategoryName { get; private set; }
        public string CounterName { get; private set; }
	    public string BaseCounterName { get; private set; }
        public PerformanceCounterType CounterType { get; private set; }
        public PerformanceCounterType? BaseCounterType { get; private set; }
	    [NonSerialized] private PerformanceCounter performanceCounter;
		public PerformanceCounter PerformanceCounter { get { return performanceCounter; } }
		[NonSerialized]
		private PerformanceCounter basePerformanceCounter;
		public PerformanceCounter BasePerformanceCounter { get { return basePerformanceCounter; } }
		public PerformanceCounterCategoryType CategoryType { get; set; }

		// Static field containing data that are shared among all aspects of an assembly and must be
		// serialized in the assembly.
		private static readonly SharedData sharedData = new SharedData();
		// We need a reference to shared data as an instance field to make sure they are serialized.
		private readonly SharedData sharedDataRef;
		#endregion

		#region ctor

	    protected PerformanceCounterAttribute(string categoryName, string counterName, PerformanceCounterType counterType)
			:this(categoryName, counterName, counterType, null)
	    {
	    }

	    protected PerformanceCounterAttribute(
			string categoryName, string counterName, PerformanceCounterType counterType, PerformanceCounterType? baseCounterType)
	    {
		    this.CategoryName = categoryName;
		    this.CounterName = counterName;
		    this.BaseCounterName = counterName + "Base";
		    this.CounterType = counterType;
		    this.BaseCounterType = baseCounterType;
			this.CategoryType=PerformanceCounterCategoryType.MultiInstance;
		    this.sharedDataRef = sharedData;
	    }
		#endregion

		#region overrides

	    public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
	    {
		    PerformanceCounterCategoryData categoryData;
		    if (!sharedDataRef.Categories.TryGetValue(this.CategoryName, out categoryData))
		    {
			    categoryData = new PerformanceCounterCategoryData() {CategoryType=this.CategoryType};
				sharedDataRef.Categories.Add(this.CategoryName, categoryData);
		    }
			else if (categoryData.CategoryType != this.CategoryType)
			{
				// todo: emit error
			}
		    categoryData.Counters.Add(new CounterCreationData(this.CounterName, "", this.CounterType));
		    if (this.BaseCounterType.HasValue)
		    {
			    categoryData.Counters.Add(new CounterCreationData(this.BaseCounterName, "", this.BaseCounterType.Value));
		    }
	    }

	    public override void RuntimeInitialize(MethodBase method)
	    {
			this.InitializeCounters();
		    this.performanceCounter = CreatePerformanceCounter(
				this.CategoryName, this.CounterName, this.CategoryType, false);
		    if (this.BaseCounterType.HasValue)
		    {
			    basePerformanceCounter = CreatePerformanceCounter(
					this.CategoryName, this.BaseCounterName, this.CategoryType, false);
		    }
	    }

		#endregion

		#region private methods

	    private void InitializeCounters()
	    {
		    switch (this.sharedDataRef.Status)
		    {
			    case PerformanceCounterCategoryStatus.Uninitialized:
					try
					{
						foreach (KeyValuePair<string, PerformanceCounterCategoryData> pair in this.sharedDataRef.Categories)
						{
							if (!PerformanceCounterCategory.Exists(pair.Key))
							{
								Trace.TraceInformation("Creating performance counters for category '{0}'.", pair.Key);
								PerformanceCounterCategory.Create(pair.Key, "", pair.Value.CategoryType, pair.Value.Counters);
							}
							else
							{
								PerformanceCounterCategory category = new PerformanceCounterCategory(pair.Key);
								bool invalid = category.CategoryType != pair.Value.CategoryType;

								foreach (CounterCreationData counterData in pair.Value.Counters)
								{
									if (!category.CounterExists(counterData.CounterName))
									{
										invalid = true;
										break;
									}

									PerformanceCounter counter = CreatePerformanceCounter(pair.Key, counterData.CounterName, pair.Value.CategoryType, true);
									if (counter.CounterType != counterData.CounterType)
									{
										invalid = true;
										break;
									}
								}

								if (invalid)
								{
									Trace.TraceInformation("Recreating performance counters for category '{0}'.", pair.Key);
									PerformanceCounterCategory.Delete(pair.Key);
									PerformanceCounterCategory.Create(pair.Key, "", pair.Value.CategoryType, pair.Value.Counters);
								}
							}
						}
					}
					catch (Exception e)
					{
						Trace.TraceError("Cannot initialize performance counter {0}.{1}: {2}",
										  this.CategoryName, this.CounterName, e.Message);
						this.sharedDataRef.Status = PerformanceCounterCategoryStatus.Failed;
						return;
					}
					this.sharedDataRef.Status = PerformanceCounterCategoryStatus.Initialized;
					break;
				case PerformanceCounterCategoryStatus.Failed:
				    return;
		    }
	    }

	    private static PerformanceCounter CreatePerformanceCounter(
			string categoryName, string counterName,
		    PerformanceCounterCategoryType categoryType, bool isReadOnly)
	    {
		    if (categoryType == PerformanceCounterCategoryType.MultiInstance)
		    {
			    return new PerformanceCounter(categoryName, counterName, PerformanceCounterSettings.InstanceName, isReadOnly);
		    }
		    else
		    {
			    return new PerformanceCounter(categoryName, counterName, isReadOnly);
		    }
	    }

		#endregion

		#region Nested types

		[Serializable]
		private class SharedData
		{
			public PerformanceCounterCategoryStatus Status { get; set; }
			private readonly Dictionary<string, PerformanceCounterCategoryData> categories = new Dictionary<string, PerformanceCounterCategoryData>();

			public Dictionary<string, PerformanceCounterCategoryData> Categories
			{
				get { return categories; }
			}
		}

		[Serializable]
		private class PerformanceCounterCategoryData
		{
			public PerformanceCounterCategoryData()
			{
				this.Counters = new CounterCreationDataCollection();
			}

			public PerformanceCounterCategoryType CategoryType { get; set; }
			public CounterCreationDataCollection Counters { get; private set; }
		}

		private enum PerformanceCounterCategoryStatus
		{
			Uninitialized,
			Initialized,
			Failed
		}

		#endregion
	}

	public static class PerformanceCounterSettings
	{
		private static string instanceName = Process.GetCurrentProcess().ProcessName;

		public static string InstanceName
		{
			get { return instanceName; }
			set { instanceName = value; }
		}
	}
}
