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
            _config.Register<IBarService, BarImplementation>(true);
            _config.Register<IBazService<BarImplementation>, AnotherBazImplementation<BarImplementation>>(false);

            _provider = new DependencyProvider(_config);
        }

        [TestMethod]
        public void DependencyProviderEnumerableTest()
        {
            var fooService = _provider.Resolve<IEnumerable<IFooService>>();
            Assert.IsTrue(fooService as IEnumerable<IFooService> != null);
        }

        [TestMethod]
        public void DependencyProviderAnotherBazImplementationTest()
        {
            AnotherBazImplementation<BarImplementation> anotherBaz =
                (AnotherBazImplementation<BarImplementation>)_provider.Resolve<IBazService<BarImplementation>>();

            bool isAnotherBazImplementation = anotherBaz.GetType().Equals(typeof(AnotherBazImplementation<BarImplementation>));

            bool isBarImplementation = anotherBaz.BarService.GetType().Equals(typeof(BarImplementation));

            Assert.IsTrue(isAnotherBazImplementation && isBarImplementation);
        }

        [TestMethod]
        public void DependencyProviderEnumerableImplementationsTest()
        {
            FooImplementation<BarImplementation> foo= 
                (FooImplementation<BarImplementation>)((List<IFooService>)_provider.Resolve<IEnumerable<IFooService>>())[0];

            bool isFooBarImplementation = foo.BarService.GetType().Equals(typeof(BarImplementation));

            bool isAnotherBazImplementation = foo.BazService.GetType().Equals(typeof(AnotherBazImplementation<BarImplementation>));

            AnotherFooImplementation anotherFoo =
                (AnotherFooImplementation)((List<IFooService>)_provider.Resolve<IEnumerable<IFooService>>())[1];

            bool isAnotheFooBarImplementation = anotherFoo.BarService.GetType().Equals(typeof(BarImplementation));

            Assert.IsTrue(isFooBarImplementation && isAnotherBazImplementation && isAnotheFooBarImplementation);
        }

        [TestMethod]
        public void DependencyProviderBarImplementationTest()
        {
            var actual = _provider.Resolve<IBarService>();
            Assert.IsTrue(actual.GetType().Equals(typeof(BarImplementation)));
        }

        [TestMethod]
        public void DependencyProviderSingletonContainerTest()
        {
            var actual = _provider.Resolve<IBarService>();
            Assert.IsTrue(actual != null);
        }

        [TestMethod]
        public void DependencyProviderSingletonBarImplementationTest()
        {
            AnotherBazImplementation<BarImplementation> anotherBaz =
                (AnotherBazImplementation<BarImplementation>)_provider.Resolve<IBazService<BarImplementation>>();

            BarImplementation bar = (BarImplementation)_provider.Resolve<IBarService>();

            Assert.IsTrue(anotherBaz.BarService == bar);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderIEnumerableExeptionTest()
        {
            _provider.Resolve<IFooService>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderIEnumerableCountExeptionTest()
        {
            _provider.Resolve<IEnumerable<IBarService>>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderNotRegisterDependencyExeptionTest()
        {
            _provider.Resolve<IFaker>();
        }
    }
}
