namespace Core.Instrumentation.Caching
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class ContextCacheNotFoundException : Exception
	{
		private const string DEFAULT_MESSAGE = "No cacheable context or cache dictionary is present";

		public ContextCacheNotFoundException() : this(DEFAULT_MESSAGE) { }
		public ContextCacheNotFoundException(string message) : base(message) { }
		public ContextCacheNotFoundException(string message, Exception innerException) : base(message, innerException) { }
		protected ContextCacheNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
