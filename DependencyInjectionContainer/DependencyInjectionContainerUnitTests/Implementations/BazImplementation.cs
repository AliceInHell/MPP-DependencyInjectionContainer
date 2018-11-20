using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyInjectionContainerUnitTests.Interfaces;

namespace DependencyInjectionContainerUnitTests.Implementations
{
    public class BazImplementation<T> : IBazService<T>
    {
        public IBarService FirstBarService { get; set; }

        public IBarService SecondBarService { get; set; }

        public BazImplementation(IBarService firstBarService, IBarService secondBarService)
        {
            FirstBarService = firstBarService;
            SecondBarService = secondBarService;
        }
    }
}
