namespace Core.Instrumentation.Caching
{
	using System;
	using System.Collections.Generic;

	public interface ICacheContext : IDisposable
	{
		IDictionary<string, object> CacheHolder
		{
			get;
		}
	}
}
