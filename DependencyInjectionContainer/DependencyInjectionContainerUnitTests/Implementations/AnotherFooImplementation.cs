using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyInjectionContainerUnitTests.Interfaces;

namespace DependencyInjectionContainerUnitTests.Implementations
{
    public class AnotherFooImplementation : IFooService
    {
        public IBarService BarService { get; set; }

        public AnotherFooImplementation(IBarService barService)
        {
            BarService = barService;
        }
    }
}
