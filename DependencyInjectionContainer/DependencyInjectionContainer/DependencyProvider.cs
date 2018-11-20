using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionContainer
{
    /// <summary>
    /// Provide Dependencies
    /// </summary>
    public class DependencyProvider
    {
        /// <summary>
        /// Dependency config
        /// </summary>
        private readonly DependencyProviderConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyProvider"/> class
        /// </summary>
        /// <param name="config">Dependency config</param>
        public DependencyProvider(DependencyProviderConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Create container
        /// </summary>
        /// <typeparam name="TImplementation">Implementation to create</typeparam>
        /// <returns>New container</returns>
        public IEnumerable<TImplementation> Resolve<TImplementation>()
        {
            if (_config.Dependencies.ContainsKey(typeof(TImplementation)))
            {
                List<TImplementation> result = new List<TImplementation>();

                foreach (var item in _config.Dependencies)
                {
                    result.Add((TImplementation)Create(typeof(TImplementation)));
                }

                return result.AsEnumerable<TImplementation>();
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Cant create an instance of{0}", typeof(TImplementation)));
            }
        }

        /// <summary>
        /// Creation logic
        /// </summary>
        /// <typeparam name="TImplementation">Creating type</typeparam>
        /// <returns>Created implementation</returns>
        private object Create(Type implementation)
        {
            ConstructorInfo[] constructors = implementation.GetConstructors();

            ConstructorInfo constructor = constructors                
                .OrderBy(constr => constr.GetParameters().Length)
                .Last();            
            
            if (constructor != null)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                object[] tmp = new object[parameters.Length];
                int i = 0;

                foreach (ParameterInfo pi in parameters)
                {
                    if (_config.Dependencies.ContainsKey(pi.GetType()))
                    {
                        tmp[i++] = Create(pi.ParameterType);
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format(
                            "Cant create an instance of{0}", implementation));
                    }                    
                }

                return constructor.Invoke(tmp);
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Cant create an instance of{0}", implementation));
            }
        }
    }
}
