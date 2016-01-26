using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IoC
{
    using Microsoft.Practices.Unity;

    /// <summary>
	/// Concrete implementation of DI based on Unity
	/// </summary>
	public class UnityDependencyInjection : IDependencyInjection
    {

        #region Declarations
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate - will create new Unity Container
        /// </summary>
        public UnityDependencyInjection()
            : this(new UnityContainer())
        {
        }

        // ReSharper disable MemberCanBePrivate.Global for future expansion
        /// <summary>
        /// Instantiate using existing container based on Unity interface
        /// </summary>
        /// <param name="container"></param>
        public UnityDependencyInjection(IUnityContainer container)
        {
            _container = container;
        }
        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        #region Public routines

        /// <summary>
        /// Register factory
        /// </summary>
        /// <typeparam name="TFactoryResult">Factory Result</typeparam>
        /// <param name="factoryFunc">Function</param>
        /// <param name="lifetime"></param>
        public void RegisterFactory<TFactoryResult>(Func<TFactoryResult> factoryFunc, LifeTime lifetime)
        {
            _container.RegisterType<Func<TFactoryResult>>(GetLifetimeManager(lifetime),
               new InjectionFactory(c => new Func<TFactoryResult>(factoryFunc)));
        }

        /// <summary>
        /// Register factory
        /// </summary>
        /// <typeparam name="TFactoryParam">Factory function parameter</typeparam>
        /// <typeparam name="TFactoryResult">Factory Result</typeparam>
        /// <param name="factoryFunc">Function</param>
        /// <param name="lifetime"></param>
        public void RegisterFactory<TFactoryParam, TFactoryResult>(Func<TFactoryParam, TFactoryResult> factoryFunc, LifeTime lifetime)
        {
            _container.RegisterType<Func<TFactoryParam, TFactoryResult>>(GetLifetimeManager(lifetime),
               new InjectionFactory(c => new Func<TFactoryParam, TFactoryResult>(factoryFunc)));
        }

        /// <summary>
        /// Register Type
        /// </summary>
        /// <typeparam name="TInterfaceToBeResloved">Interface Type</typeparam>
        /// <typeparam name="TConcreteClass">Instance Type</typeparam>
        /// <param name="lifeTime">LifeTime Model</param>
        public void RegisterType<TInterfaceToBeResloved, TConcreteClass>(LifeTime lifeTime) where TConcreteClass : TInterfaceToBeResloved
        {
            _container.RegisterType<TInterfaceToBeResloved, TConcreteClass>(GetLifetimeManager(lifeTime));
        }

        /// <summary>
        /// Register Type
        /// </summary>
        /// <typeparam name="TInterfaceToBeResloved">Interface Type</typeparam>
        /// <typeparam name="TConcreteClass">Instance Type</typeparam>
        /// <param name="name"></param>
        /// <param name="lifeTime">LifeTime Model</param>
        public void RegisterType<TInterfaceToBeResloved, TConcreteClass>(string name, LifeTime lifeTime) where TConcreteClass : TInterfaceToBeResloved
        {
            _container.RegisterType<TInterfaceToBeResloved, TConcreteClass>(name, GetLifetimeManager(lifeTime));
        }

        /// <summary>
        /// register instance
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <param name="instance">Singleton Instance</param>
        public void RegisterInstance<TInterface>(object instance)
        {
            _container.RegisterInstance(typeof(TInterface), instance);
        }

        /// <summary>
        /// Resolve specific type/interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Resolve specific type/interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>(string name)
        {
            return _container.Resolve<T>(name);
        }

        /// <summary>
        /// Resolution using factory injection
        /// </summary>
        /// <typeparam name="TFactoryParam">Factory function parameter</typeparam>
        /// <typeparam name="TFactoryResult">Resolving type</typeparam>
        /// <param name="factoryParam"></param>
        /// <returns>Resolved instance</returns>
        public TFactoryResult Resolve<TFactoryParam, TFactoryResult>(TFactoryParam factoryParam)
        {
            var factoryFunc = _container.Resolve<Func<TFactoryParam, TFactoryResult>>();

            var resolvedInstance = factoryFunc.Invoke(factoryParam);
            TFactoryResult returnValue;
            try
            {
                returnValue = (TFactoryResult)_container.BuildUp(resolvedInstance.GetType(), resolvedInstance);
            }
            catch (Exception)
            {
                //sometimes BuildUp fails - on value types and if there is no default constructor
                //TODO: Add logging
                returnValue = resolvedInstance;
            }
            return returnValue;
        }

        public bool IsRegistered<T>()
        {
            return _container.IsRegistered<T>();
        }
        #endregion

        #region Private routines
        private LifetimeManager GetLifetimeManager(LifeTime lifetime)
        {
            LifetimeManager lifetimeManager = null;
            switch (lifetime)
            {
                case LifeTime.Transient:
                    lifetimeManager = new PerResolveLifetimeManager();
                    break;
                case LifeTime.SinglePerThread:
                    lifetimeManager = new PerThreadLifetimeManager();
                    break;
                case LifeTime.Singleton:
                    lifetimeManager = new ContainerControlledLifetimeManager();
                    break;
            }
            return lifetimeManager;
        }
        #endregion

    }
}
