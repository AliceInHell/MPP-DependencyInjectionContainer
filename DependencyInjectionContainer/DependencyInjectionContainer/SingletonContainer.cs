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
    public sealed class SingletonContainer
    {
        #region Fields

        /// <summary>
        /// Object-locker
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Lock property
        /// </summary>
        private readonly object _boolLocker = new object();

        /// <summary>
        /// Instance
        /// </summary>
        private volatile object _instance = null;

        /// <summary>
        /// Instance type
        /// </summary>
        private Type _instanceType;

        /// <summary>
        /// Status
        /// </summary>
        private bool _isBusy;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonContainer{T}"/> class
        /// </summary>
        public SingletonContainer(Type instanceType)
        {            
            _instanceType = instanceType;
        }

        /// <summary>
        /// Check if we creating instance now
        /// </summary>
        public bool IsBusy
        {
            get
            {
                lock (_boolLocker)
                {
                    return _isBusy;
                }                
            }

            private set
            {
                _isBusy = value;
            }            
        }

        /// <summary>
        /// Create singleton instance or return it
        /// </summary>
        /// <param name="provider">Instance provider</param>
        /// <returns>Single instance</returns>
        public object GetInstance(DependencyProvider provider)
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        lock (_boolLocker)
                        {
                            IsBusy = true;
                        }

                        _instance = provider.Resolve(_instanceType);

                        lock (_boolLocker)
                        {
                            IsBusy = false;
                        }
                    }
                }
            }

            return _instance;
        }
    }
}
