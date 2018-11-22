using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    /// <summary>
    /// Singleton container
    /// </summary>
    /// <typeparam name="TDependency">Instance type</typeparam>
    public sealed class SingletonContainer<TDependency> where TDependency : class
    {
        /// <summary>
        /// Object-locker
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// Instance
        /// </summary>
        private static volatile TDependency _instance = null;        

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonContainer{T}"/> class
        /// </summary>
        public SingletonContainer()
        {
        }

        /// <summary>
        /// Create singleton instance or return it
        /// </summary>
        /// <param name="provider">Instance provider</param>
        /// <returns>Single instance</returns>
        public static TDependency GetInstance(DependencyProvider provider)
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = provider.Resolve<TDependency>();
                    }
                }
            }

            return _instance;
        }
    }
}
