using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyInjectionContainer
{
    /// <summary>
    /// Dependency provider configuration
    /// </summary>
    public class DependencyProviderConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyProviderConfiguration"/> class
        /// </summary>
        public DependencyProviderConfiguration()
        {
            Dependencies = new Dictionary<Type, List<Type>>();
            IsSingleton = new Dictionary<Type, bool>();
            Singletons = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Dependency container
        /// </summary>
        internal Dictionary<Type, List<Type>> Dependencies { get; set; }

        /// <summary>
        /// Check implementations for singleton
        /// </summary>
        internal Dictionary<Type, bool> IsSingleton { get; set; }

        /// <summary>
        /// Contains singletons
        /// </summary>
        internal Dictionary<Type, object> Singletons { get; set; }

        /// <summary>
        /// Register dependency
        /// </summary>
        /// <typeparam name="TDependency">Dependency interface</typeparam>
        /// <typeparam name="TImplementation">Dependency implementation</typeparam>
        public void Register<TDependency, TImplementation>(bool isSingleton) 
            where TDependency : class where TImplementation : class, TDependency
        {
            if (typeof(TDependency).IsInterface)
            {
                RegisterDependency(typeof(TDependency), typeof(TImplementation), isSingleton);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("Cant register {0}", typeof(TDependency)));
            }
        }

        /// <summary>
        /// Register open generics
        /// </summary>
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <param name="isSingleton">True if instance is singleton</param>
        public void Register(Type dependencyType, Type implementationType, bool isSingleton)
        {
            if (dependencyType.IsGenericType)               
            {
                RegisterDependency(dependencyType, implementationType, isSingleton);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("Cant register {0}", dependencyType));
            }
        }

        /// <summary>
        /// Registration realization
        /// </summary>
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <param name="isSingleton">True if instance is singleton</param>
        private void RegisterDependency(Type dependencyType, Type implementationType, bool isSingleton)
        {            
            if (!Dependencies.ContainsKey(dependencyType))
            {
                Dependencies[dependencyType] = new List<Type>();
            }

            if (Dependencies[dependencyType].IndexOf(implementationType) == -1)
            {
                Dependencies[dependencyType].Add(implementationType);

                IsSingleton[dependencyType] = isSingleton;

                if (isSingleton)
                {                    
                    if (!IsSingleton.ContainsKey(implementationType))
                    {
                        IsSingleton[implementationType] = isSingleton;
                        Singletons[implementationType] = new SingletonContainer(dependencyType);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            string.Format("Cant register {0}", dependencyType));
                    }
                }
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("Cant register {0}", dependencyType));
            }
        }
    }
}
