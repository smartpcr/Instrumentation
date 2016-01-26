using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Tests.Hooks
{
    using Core.Instrumentation.Tests.Mocks;
    using Core.Instrumentation.Tracings;
    using Core.IoC;
    using TechTalk.SpecFlow;

    [Binding]
    public class Bootstrap
    {
        internal static IDependencyInjection DI = new UnityDependencyInjection();

        [BeforeTestRun]
        public static void SetupContainer()
        {
            IoCFacade.InitializeContainer(DI,SetupMocksAndRegisterTypes);
        }

        private static void SetupMocksAndRegisterTypes(IDependencyInjection resolver)
        {
//            var loggerMock = MockFactory.GetTraceLogger();
//            resolver.RegisterInstance<ITraceLogger>(loggerMock);
        }
    }
}
