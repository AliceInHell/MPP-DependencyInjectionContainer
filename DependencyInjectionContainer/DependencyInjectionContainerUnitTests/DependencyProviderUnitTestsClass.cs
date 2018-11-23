using System;
using System.Collections.Generic;
using DependencyInjectionContainer;
using DependencyInjectionContainerUnitTests.Implementations;
using DependencyInjectionContainerUnitTests.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
            _config.Register<IFooService, FooImplementation<BarImplementation>>(isSingleton: false);
            _config.Register<IFooService, AnotherFooImplementation>(false);
            _config.Register<IBarService, BarImplementation>(true);
            _config.Register<IBazService<BarImplementation>, AnotherBazImplementation<BarImplementation>>(false);
            _config.Register(typeof(IBazService<>), typeof(AnotherBazImplementation<>), false);

            _provider = new DependencyProvider(_config);
        }

        [TestMethod]
        public void DependencyProviderEnumerableTest()
        {
            IEnumerable<IFooService> fooService = _provider.Resolve<IEnumerable<IFooService>>();
            Assert.IsTrue(fooService != null);
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
            var impls = _provider.Resolve<IEnumerable<IFooService>>().ToArray();
            IFooService fooImplementation = impls[0];
            IFooService anotherFooImplementation = impls[1];

            Assert.IsInstanceOfType(((FooImplementation<BarImplementation>)fooImplementation).BarService, typeof(BarImplementation));
            Assert.IsInstanceOfType(((FooImplementation<BarImplementation>)fooImplementation).BazService, typeof(AnotherBazImplementation<BarImplementation>));
            Assert.IsInstanceOfType(anotherFooImplementation, typeof(AnotherFooImplementation));
        }

        [TestMethod]
        public void DependencyProviderBarImplementationTest()
        {
            var actual = _provider.Resolve<IBarService>();
            Assert.IsInstanceOfType(actual, (typeof(BarImplementation)));
        }

        [TestMethod]
        public void DependencyProviderSingletonContainerTest()
        {
            var actual = _provider.Resolve<IBarService>();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void DependencyProviderSingletonBarImplementationTest()
        {
            AnotherBazImplementation<BarImplementation> anotherBaz =
                (AnotherBazImplementation<BarImplementation>)_provider.Resolve<IBazService<BarImplementation>>();

            BarImplementation bar = (BarImplementation)_provider.Resolve<IBarService>();

            Assert.AreSame(anotherBaz.BarService, bar);
        }

        [TestMethod]
        public void DependencyInjectionOpenGenericTest()
        {
            var openGeneric = _provider.Resolve<IBazService<BarImplementation>>();
            Assert.IsNotNull(openGeneric);
            Assert.IsInstanceOfType(
                ((AnotherBazImplementation<BarImplementation>)openGeneric).BarService,
                typeof(BarImplementation));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderIEnumerableExceptionTest()
        {
            _provider.Resolve<IFooService>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderIEnumerableCountExceptionTest()
        {
            _provider.Resolve<IEnumerable<IBarService>>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderNotRegisterDependencyExceptionTest()
        {
            _provider.Resolve<IFaker>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderRegisterContainingDependencyExceptionTest()
        {
            _config.Register<IFooService, AnotherFooImplementation>(false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyProviderNotInterfaceRegistrarionExceptionTest()
        {
            _config.Register<object, AnotherFooImplementation>(false);
        }        
    }
}
