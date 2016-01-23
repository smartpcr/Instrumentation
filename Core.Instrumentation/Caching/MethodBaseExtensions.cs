using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Caching
{
	using System.Reflection;
	using PostSharp.Aspects;

	public static class MethodBaseExtensions
	{
		public static bool Implements<T>(this MethodBase method)
		{
			return (method.ReflectedType.GetInterfaceMap(typeof(T)).TargetMethods.Contains(method));
		}

		public static bool IsOfType<T>(this ParameterInfo parameter)
		{
			return
				typeof(T).IsAssignableFrom(parameter.ParameterType.IsByRef
												? parameter.ParameterType.GetElementType()
												: parameter.ParameterType);
		}
		public static bool HasVoidReturn(this MethodBase method)
		{
			var methodInfo = method as MethodInfo;
			return methodInfo != null && methodInfo.ReturnType.Name == "Void";
		}

		public static bool HasOutParameter(this MethodBase method)
		{
			var parameters = method.GetParameters();
			var hasOutParameter = parameters.Any(x => x.IsOut);
			return hasOutParameter;
		}

		public static bool HasICacheContextParameter(this MethodBase method)
		{
			var parameters = method.GetParameters();
			return parameters.Any(param =>
				typeof(ICacheContext)
					.IsAssignableFrom(param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType)
			);
		}

		public static bool IsConstructorInfo(this MethodBase method)
		{
			var isConstructor = method != null && method is ConstructorInfo;
			return isConstructor;
		}

		public static IDictionary<string, object> GetCacheDictionary(this MethodExecutionArgs args)
		{
			var cacheContext =
					(ICacheContext)
						args
							.Arguments
							.FirstOrDefault(param => param is ICacheContext);

			return cacheContext == null
				? null
				: cacheContext.CacheHolder;
		}
	}
}
