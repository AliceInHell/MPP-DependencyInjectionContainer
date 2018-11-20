using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyInjectionContainerUnitTests.Interfaces;

namespace DependencyInjectionContainerUnitTests.Implementations
{
    public class FooImplementation<T> : IFooService
    {
        public IBarService BarService { get; set; }

        public IBazService<T> BazService { get; set; }

        public FooImplementation(IBarService barService, IBazService<T> bazService)
        {
            BarService = barService;
            BazService = bazService;
        }
    }
}
