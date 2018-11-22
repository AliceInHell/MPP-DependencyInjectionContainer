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
        public TDependency Resolve<TDependency>() where TDependency : class
        {          
            return (TDependency)Resolve(typeof(TDependency));
        }

        /// <summary>
        /// Deside is it an IEnumerable<>, creating implementation
        /// </summary>
        /// <param name="t">Implementaiton type</param>
        /// <returns>Implementation</returns>
        internal object Resolve(Type t)
        {
            if (_config.Dependencies.ContainsKey(t) && _config.Dependencies[t].Count == 1)
            {
                if (_config.IsSingleton.ContainsKey(_config.Dependencies[t][0]))
                {
                    if (_config.IsSingleton[_config.Dependencies[t][0]] 
                        && !((SingletonContainer)(_config.Singletons[_config.Dependencies[t][0]])).IsBusy)
                    {
                        return ((SingletonContainer)(_config.Singletons[_config.Dependencies[t][0]])).GetInstance(this);
                    }
                }
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                if (_config.Dependencies.ContainsKey(t.GenericTypeArguments[0]) &&
                    _config.Dependencies[t.GenericTypeArguments[0]].Count > 1)
                {
                    object result = Activator.CreateInstance(typeof(List<>).MakeGenericType(t.GenericTypeArguments[0]));

                    foreach (Type item in _config.Dependencies[t.GenericTypeArguments[0]])
                    {
                        ((IList)result).Add(Create(item));
                    }

                    return result;
                }
            }
            else
            {
                if (t.IsGenericType)
                {
                    if (_config.Dependencies.ContainsKey(t.GetGenericTypeDefinition()) &&
                        _config.Dependencies.ContainsKey(t.GenericTypeArguments[0]))
                    {
                        return Create(_config.Dependencies[t.GetGenericTypeDefinition().MakeGenericType(
                            _config.Dependencies[t.GenericTypeArguments[0]][0])][0]);
                    }
                    else
                    {
                        if (_config.Dependencies.ContainsKey(t.GetGenericTypeDefinition()) &&
                        _config.Dependencies.ContainsKey(GetDependency(t.GenericTypeArguments[0])))
                        {
                            return Create(_config.Dependencies[t][0]);
                        }
                    }
                }
                else
                {
                    if (_config.Dependencies.ContainsKey(t) && _config.Dependencies[t].Count == 1)
                    {
                        return Create(_config.Dependencies[t][0]);
                    }
                }
            }           

            throw new InvalidOperationException(string.Format(
                        "Cant create an instance of{0}", t));
        }

        /// <summary>
        /// Creation logic
        /// </summary>
        /// <typeparam name="TImplementation">Creating type</typeparam>
        /// <returns>Created implementation</returns>
        private object Create(Type t)
        {            
            if (t.IsGenericType && _config.Dependencies.ContainsKey(t.GenericTypeArguments[0]))
            {
                t = t.GetGenericTypeDefinition().MakeGenericType(_config.Dependencies[t.GenericTypeArguments[0]][0]);
            }

            ConstructorInfo[] constructors = t.GetConstructors();

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
                        if (_config.Dependencies.ContainsKey(pi.ParameterType.GetGenericTypeDefinition()) &&
                            _config.Dependencies.ContainsKey(GetDependency(pi.ParameterType.GenericTypeArguments[0])))                     
                        {
                            tmp[i++] = Resolve(pi.ParameterType.GetGenericTypeDefinition()
                                .MakeGenericType(GetDependency(pi.ParameterType.GenericTypeArguments[0])));                            
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(
                                "Cant create an instance of{0}", pi.ParameterType));
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
                                "Cant create an instance of{0}", pi.ParameterType));
                        }
                    }
                }

                var result = constructor.Invoke(tmp);
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Cant create an instance of{0}", t));
            }
        }

        /// <summary>
        /// Get key by value from Dependencies
        /// </summary>
        /// <param name="t">Implementation type</param>
        /// <returns>Dependency type</returns>
        private Type GetDependency(Type t)
        {
            return _config.Dependencies.First(x => x.Value[0].Equals(t)).Key; 

            /*foreach(var item in _config.Dependencies)
            {
                foreach(Type element in item.Value)
                {
                    if (element.Equals(t))
                    {
                        return item.Key;
                    }
                }
            }*/
        }        
    }
}
