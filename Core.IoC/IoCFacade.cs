using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IoC
{
    /// <summary>
	/// Implementation-agnostic IoC facade
	/// </summary>
	public static class IoCFacade
    {
        private static IDependencyInjection _resolver;

        /// <summary>
        /// Initialize container with concrete DI implementer and bootstrap routine to register with DI container
        /// </summary>
        /// <param name="resolver">concrete DI implementer</param>
        /// <param name="bootStrap">routine to register with DI container</param>
        public static void InitializeContainer(IDependencyInjection resolver, Action<IDependencyInjection> bootStrap)
        {
            _resolver = resolver;
            bootStrap(_resolver);
        }

        /// <summary>
        /// Resolve specific type/interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _resolver.Resolve<T>();
        }

        /// <summary>
        /// Resolution using factory injection
        /// </summary>
        /// <typeparam name="TFactoryParam">Factory function parameter</typeparam>
        /// <typeparam name="TFactoryResult">Resolving type</typeparam>
        /// <param name="factoryParam"></param>
        /// <returns>Resolved instance</returns>
        public static TFactoryResult Resolve<TFactoryParam, TFactoryResult>(TFactoryParam factoryParam)
        {
            return _resolver.Resolve<TFactoryParam, TFactoryResult>(factoryParam);
        }

        /// <summary>
        /// Register factory
        /// </summary>
        /// <typeparam name="TFactoryParam">Factory function parameter</typeparam>
        /// <typeparam name="TFactoryResult">Factory Result</typeparam>
        /// <param name="factoryFunc">Function</param>
        /// <param name="lifetime"></param>
        public static void RegisterFactory<TFactoryParam, TFactoryResult>(Func<TFactoryParam, TFactoryResult> factoryFunc, LifeTime lifetime)
        {
            _resolver.RegisterFactory(factoryFunc, lifetime);
        }
    }
}
