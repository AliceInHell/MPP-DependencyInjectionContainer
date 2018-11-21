using System;
using System.Collections.Generic;
using DependencyInjectionContainer;
using DependencyInjectionContainerUnitTests.Implementations;
using DependencyInjectionContainerUnitTests.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjectionContainerUnitTests
{
    [TestClass]
    public class DependencyProviderUnitTestsClass
    {
        private DependencyProvider _provider;
        private DependencyProviderConfiguration _config;        

        [TestInitialize]
        public void Initialize()
        {
            _config = new DependencyProviderConfiguration();
            _config.Register<IFooService, FooImplementation<BarImplementation>>(false);
            _config.Register<IFooService, AnotherFooImplementation>(false);
            _config.Register<IBarService, BarImplementation>(false);
            _config.Register<IBazService<IBarService>, AnotherBazImplementation<IBarService>>(false);

            _provider = new DependencyProvider(_config);
        }

        [TestMethod]
        public void DependencyProviderEnumerableTest()
        {
            Assert.IsTrue(_provider.Resolve<IEnumerable<IFooService>>() is IEnumerable<IFooService>);
        }
    }
}
