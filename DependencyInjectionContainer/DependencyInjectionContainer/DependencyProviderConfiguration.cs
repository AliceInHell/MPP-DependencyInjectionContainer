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
        /// Register dependency
        /// </summary>
        /// <typeparam name="TDependency">Dependency interface</typeparam>
        /// <typeparam name="TImplementation">Dependency implementation</typeparam>
        public void Register<TDependency, TImplementation>(bool isSingleton) 
            where TDependency : class where TImplementation : TDependency
        {
            if (!Dependencies.ContainsKey(typeof(TDependency)))
            {            
                Dependencies[typeof(TDependency)] = new List<Type>();
            }

            Dependencies[typeof(TDependency)].Add(typeof(TImplementation));

            IsSingleton[typeof(TDependency)] = isSingleton;
        }
    }
}
