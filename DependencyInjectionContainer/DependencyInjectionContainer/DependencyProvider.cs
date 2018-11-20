using System;
using System.Collections;
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
        /// Create a container
        /// </summary>
        /// <typeparam name="TDependency">Implementation to create</typeparam>
        /// <returns>New container</returns>
        public TDependency Resolve<TDependency>()
        {
            return (TDependency)Resolve(typeof(TDependency));
        }

        private object Resolve(Type t)
        {
            if (t.IsGenericType && t is IEnumerable)
            {
                if (_config.Dependencies.ContainsKey(t.GenericTypeArguments[0].GetType()))
                {
                    object result = Activator.CreateInstance(typeof(List<>).MakeGenericType(t.GenericTypeArguments[0]));

                    foreach (Type item in _config.Dependencies[t])
                    {
                        ((IList)result).Add(Create(item));
                    }

                    return result;
                }
                else
                {
                    throw new InvalidOperationException(string.Format(
                        "Cant create an instance of{0}", t));
                }
            }
            else
            {
                if (_config.Dependencies.ContainsKey(t))
                {
                    return Create(_config.Dependencies[t][0]);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(
                        "Cant create an instance of{0}", t));
                }
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
                    if (pi.ParameterType.IsGenericType)
                    {
                        if (_config.Dependencies.ContainsKey(pi.ParameterType.GetGenericTypeDefinition()))                        
                        {
                            tmp[i++] = Resolve(pi.ParameterType.GetGenericTypeDefinition()
                                .MakeGenericType(GetDependency(pi.ParameterType.GenericTypeArguments[0])));                            
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(
                                "Cant create an instance of{0}", implementation));
                        }
                    }
                    else
                    {
                        if (_config.Dependencies.ContainsKey(pi.ParameterType))
                        {
                            tmp[i++] = Resolve(pi.ParameterType);
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(
                                "Cant create an instance of{0}", implementation));
                        }
                    }
                }

                var result = constructor.Invoke(tmp);
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Cant create an instance of{0}", implementation));
            }
        }

        private Type GetDependency(Type t)
        {
            foreach(var item in _config.Dependencies)
            {
                foreach(Type element in item.Value)
                {
                    if (element.Equals(t))
                    {
                        return item.Key;
                    }
                }
            }

            throw new InvalidOperationException(string.Format(
                    "Cant create an instance of{0}", t));
        }        
    }
}
