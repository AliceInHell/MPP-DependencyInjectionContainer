using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyInjectionContainerUnitTests.Interfaces;

namespace DependencyInjectionContainerUnitTests.Implementations
{
    public class AnotherBarImplementation : IBarService
    {
        public AnotherBarImplementation(IBarService barService) { }
    }
}
