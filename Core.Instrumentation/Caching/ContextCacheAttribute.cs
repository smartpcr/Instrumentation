namespace Core.Instrumentation.Caching
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using PostSharp;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[Serializable]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ContextCacheAttribute : OnMethodBoundaryAspect
	{
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
				Message.Write(MessageLocation.Of(method), SeverityType.Error, "CX0003", "Cannot cache methods with \"out\" parameters");
				return false;
			}

			if (!method.HasICacheContextParameter())
			{
				Message.Write(MessageLocation.Of(method), SeverityType.Error, "CX0004", "Cannot cache methods without ICacheContext parameters.");
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
			var cache = eventArgs.GetCacheDictionary();

			// Compose the cache key.
			// Ignore ICacheContext every time
			List<object> args = new List<object>();
			foreach (object arg in eventArgs.Arguments)
			{
				if (arg is ICacheContext)
				{
					args.Add(null);
				}
				else
				{
					args.Add(arg);
				}
			}
			string key = this.formatStrings.Format(
				eventArgs.Instance, eventArgs.Method, args.ToArray());

			// Check if cachable param has been detected
			if (cache == null)
			{
				//Preserve the key in case cache gets created prior to exiting the method
				eventArgs.MethodExecutionTag = key;
			}
			else
			{
				// Test whether the cache contains the current method call.
				lock (cache)
				{
					object value;
					if (!cache.TryGetValue(key, out value))
					{
						// If not, we will continue the execution as normally.
						// We store the key in a state variable to have it in the OnExit method.
						eventArgs.MethodExecutionTag = key;
					}
					else
					{
						// If it is in cache, we set the cached value as the return value
						// and we force the method to return immediately.
						eventArgs.ReturnValue = value;
						eventArgs.FlowBehavior = FlowBehavior.Return;
					}
				}
			}
		}

		// Executed at runtime, after the method.
		public override void OnSuccess(MethodExecutionArgs eventArgs)
		{
			var cache = eventArgs.GetCacheDictionary();
			if (cache != null)
			{
				// Retrieve the key that has been computed in OnEntry.
				string key = (string)eventArgs.MethodExecutionTag;

				// Put the return value in the cache.
				lock (cache)
				{
					cache[key] = eventArgs.ReturnValue;
				}
			}
			else
			{
				throw new InvalidOperationException("No cacheable context or cache dictionary is present");
			}
		}
	}
}
