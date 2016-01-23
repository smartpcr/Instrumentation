namespace Core.Instrumentation.Caching
{
	using System;
	using PostSharp.Aspects;

	[Serializable]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ContextCacheAttribute : OnMethodBoundaryAspect
	{
		private MethodFormatStrings formatStrings;
	}
}
