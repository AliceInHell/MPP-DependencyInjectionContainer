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

        public IBarService BarService { get; set; }

        public AnotherBazImplementation(IBarService barService)
        {
            BarService = barService;
        }
    }
}
