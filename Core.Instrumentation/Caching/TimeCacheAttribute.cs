namespace Core.Instrumentation.Caching
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;
	using PostSharp;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	/// <summary>
	/// Aspect that, when applied on a method, emits a trace message before and
	/// after the method execution.
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class TimeCacheAttribute : OnMethodBoundaryAspect
	{
		private class TimedStorage
		{
			public TimedStorage(object storedValue)
			{
				TimeStamp = DateTime.Now;
				Storage = storedValue;
			}

			public DateTime TimeStamp { get; set; }

			public object Storage { get; set; }
		}

		/// <summary>
		/// Construct TimeCacheAttribute and define TTL
		/// </summary>
		/// <param name="minutesToLive"></param>
		public TimeCacheAttribute(double minutesToLive)
		{
			this.MinutesToLive = minutesToLive;

		}

		// A dictionary that serves as a trivial cache implementation.
		[NonSerialized]
		private static IDictionary<string, TimedStorage> cache;

		//Multiplier factor to calculate time to live
		[NonSerialized]
		private static double multiplierFactor;

		/// <summary>
		/// Static constructor to initialize non-serializable static members
		/// </summary>
		static TimeCacheAttribute()
		{
			cache = new Dictionary<string, TimedStorage>();

			//see app.config for setting - is multiplierFactor being set
			multiplierFactor = 0d;
			var appSettingsValue = ConfigurationManager.AppSettings["TimeCacheMultiplier"];
			if (string.IsNullOrWhiteSpace(appSettingsValue) || !double.TryParse(appSettingsValue, out multiplierFactor))
			{
				multiplierFactor = 1.0d;
			}
			else if (multiplierFactor < 0)
			{
				multiplierFactor = 0d;
			}
		}

		public double MinutesToLive { get; private set; }

		// Some formatting strings to compose the cache key.
		private MethodFormatStrings formatStrings;


		// Validate the attribute usage.
		public override bool CompileTimeValidate(MethodBase method)
		{
			if (method.IsConstructorInfo())
			{
				Message.Write(MessageLocation.Of(method), SeverityType.Error, "CX0001", "Cannot cache constructors.");
				return false;
			}

			if (method.HasVoidReturn())
			{
				Message.Write(MessageLocation.Of(method), SeverityType.Error, "CX0002", "Cannot cache void methods.");
				return false;
			}

			if (method.HasOutParameter())
			{
				Message.Write(MessageLocation.Of(method), SeverityType.Error, "CX0003", "Cannot cache methods with return values.");
				return false;
			}

			return true;
		}

		// At compile time, initialize the format string that will be
		// used to create the cache keys.
		public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
		{
			this.formatStrings = Formatter.GetMethodFormatStrings(method);
		}

		// Executed at runtime, before the method.
		public override void OnEntry(MethodExecutionArgs eventArgs)
		{
			// Compose the cache key.
			string key = this.formatStrings.Format(
				eventArgs.Instance, eventArgs.Method, eventArgs.Arguments.ToArray());

			// Test whether the cache contains the current method call.
			lock (cache)
			{
				TimedStorage value;
				if (!cache.TryGetValue(key, out value))
				{
					// If not, we will continue the execution as normally.
					// We store the key in a state variable to have it in the OnExit method.
					eventArgs.MethodExecutionTag = key;
				}
				else
				{
					//check ttl
					if (DateTime.Now - value.TimeStamp < TimeSpan.FromMinutes(this.MinutesToLive * multiplierFactor))
					{
						// If it is in cache, we set the cached value as the return value
						// and we force the method to return immediately.
						eventArgs.ReturnValue = value.Storage;
						eventArgs.FlowBehavior = FlowBehavior.Return;
					}
					else
					{
						//invalidate entry - it's too old
						cache.Remove(key);
						//continue on with method execution
						// We store the key in a state variable to have it in the OnExit method.
						eventArgs.MethodExecutionTag = key;
					}
				}
			}
		}

		// Executed at runtime, after the method.
		public override void OnSuccess(MethodExecutionArgs eventArgs)
		{
			// Retrieve the key that has been computed in OnEntry.
			string key = (string)eventArgs.MethodExecutionTag;

			// Put the return value in the cache.
			lock (cache)
			{
				cache[key] = new TimedStorage(eventArgs.ReturnValue);
			}
		}
	}
}
