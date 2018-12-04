using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyInjectionContainerUnitTests.Interfaces;

namespace DependencyInjectionContainerUnitTests.Implementations
{
    public class AnotherBazImplementation<T> : IBazService<T>
    {
        public T GenericParameter { get; set; }

        public T BarService { get; set; }

        public AnotherBazImplementation(T barService)
        {
            BarService = barService;
        }
    }
}
