using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IoC
{
    /// <summary>
	/// decide instance life cycle/instance management
	/// </summary>
	public enum LifeTime
    {
        /// <summary>
        /// Construct a new PerResolveLifetimeManager object that does not itself manage an instance.
        /// </summary>
        Transient,
        /// <summary>
        /// Initializes a new instance of the PerThreadLifetimeManager class.
        /// </summary>
        SinglePerThread,
        /// <summary>
        /// A LifetimeManager that holds onto the instance given to it.  When the ContainerControlledLifetimeManager is disposed, the instance is disposed with it.
        /// </summary>
        Singleton
    }

    /// <summary>
    /// interface to be implemented by concrete DI container wrappers
    /// </summary>
    public interface IDependencyInjection
    {
        /// <summary>
        /// Resolve specific type/interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary>
        /// Resolve specific type/interface
        /// </summary>
        /// <typeparam name="T">name of instance</typeparam>
        /// <returns></returns>
        T Resolve<T>(string name);

        /// <summary>
        /// Resolution using factory injection
        /// </summary>
        /// <typeparam name="TFactoryParam">Factory function parameter</typeparam>
        /// <typeparam name="TFactoryResult">Resolving type</typeparam>
        /// <param name="factoryParam"></param>
        /// <returns>Resolved instance</returns>
        TFactoryResult Resolve<TFactoryParam, TFactoryResult>(TFactoryParam factoryParam);

        /// <summary>
        /// Resolves all registered instances of a type. Used for testing and debugging.
        /// </summary>
        /// <typeparam name="T">Registered type</typeparam>
        /// <returns></returns>
        bool IsRegistered<T>();

        /// <summary>
        /// Register factory
        /// </summary>
        /// <typeparam name="TFactoryResult">Factory Result</typeparam>
        /// <param name="factoryFunc">Function</param>
        /// <param name="lifetime"></param>
        void RegisterFactory<TFactoryResult>(Func<TFactoryResult> factoryFunc, LifeTime lifetime);

        /// <summary>
        /// Register factory
        /// </summary>
        /// <typeparam name="TFactoryParam">Factory function parameter</typeparam>
        /// <typeparam name="TFactoryResult">Factory Result</typeparam>
        /// <param name="factoryFunc">Function</param>
        /// <param name="lifetime"></param>
        void RegisterFactory<TFactoryParam, TFactoryResult>(Func<TFactoryParam, TFactoryResult> factoryFunc, LifeTime lifetime);

        /// <summary>
        /// Register Type
        /// </summary>
        /// <typeparam name="TInterfaceToBeResloved">Interface Type</typeparam>
        /// <typeparam name="TConcreteClass">Instance Type</typeparam>
        /// <param name="lifeTime">LifeTime Model</param>
        void RegisterType<TInterfaceToBeResloved, TConcreteClass>(LifeTime lifeTime)
            where TConcreteClass : TInterfaceToBeResloved;

        /// <summary>
        /// Register Type
        /// </summary>
        /// <typeparam name="TInterfaceToBeResloved">Interface Type</typeparam>
        /// <typeparam name="TConcreteClass">Instance Type</typeparam>
        /// <param name="name">name of instance</param>
        /// <param name="lifeTime">LifeTime Model</param>
        void RegisterType<TInterfaceToBeResloved, TConcreteClass>(string name, LifeTime lifeTime)
            where TConcreteClass : TInterfaceToBeResloved;

        /// <summary>
        /// register instance
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <param name="instance">Singleton Instance</param>
        void RegisterInstance<TInterface>(object instance);
    }
}
