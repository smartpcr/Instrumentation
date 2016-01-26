using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Tracings
{
    public interface ITraceLogger
    {
        void BeforeMethod(string enteringMessage, Categories category, Layers layer);
        void AfterMethod(string enteringMessage, Categories category, Layers layer);
    }
}
