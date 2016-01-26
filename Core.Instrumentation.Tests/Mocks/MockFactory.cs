using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Tests.Mocks
{
    using Core.Instrumentation.Tracings;
    using Moq;

    public class MockFactory
    {
        public static ITraceLogger GetTraceLogger()
        {
            var mockObj = new Mock<ITraceLogger>();
//            mockObj.Setup(x => x.BeforeMethod(It.IsAny<string>(), It.IsAny<Categories>(), It.IsAny<Layers>()))
//                .Callback((string msg, Categories category, Layers layer) =>
//                {
//                    
//                });
//            mockObj.Setup(x => x.AfterMethod(It.IsAny<string>(), It.IsAny<Categories>(), It.IsAny<Layers>()))
//               .Callback((string msg, Categories category, Layers layer) =>
//               {
//
//               });

            return mockObj.Object;
        }
    }
}
