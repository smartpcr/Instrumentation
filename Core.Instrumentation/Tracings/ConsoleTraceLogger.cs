using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Tracings
{
    using System.Diagnostics;
    using Core.Instrumentation.Tracking;

    public class ConsoleTraceLogger : ITraceLogger
    {
        public void BeforeMethod(string message, Categories category, Layers layer)
        {
            Trace.WriteLine(message);
        }

        public void AfterMethod(string message, Categories category, Layers layer)
        {
            Trace.WriteLine(message);
        }

        public void Enter(Categories category, Layers layer, string className, string methodName, string message, string inArgs)
        {
            Trace.WriteLine(string.Format("{0}-{1}: {2}.{3}\n\t{4}", category, layer, className, methodName, message));
            if (!string.IsNullOrEmpty(inArgs))
            {
                Trace.WriteLine(string.Format("\tinput: {0}", inArgs));
            }
            else
            {
                Trace.WriteLine("\tinput: null");
            }
        }

        public void Exit(Categories category, Layers layer, string className, string methodName, string message, string outArgs, long? elapsedMiliSeconds)
        {
            Trace.WriteLine(string.Format("{0}-{1}: {2}.{3}\n\t{4}", category, layer, className, methodName, message));
            if (!string.IsNullOrEmpty(outArgs))
            {
                Trace.WriteLine(string.Format("\toutput: {0}", outArgs));
            }
            else
            {
                Trace.WriteLine("\toutput: null");
            }
            if (elapsedMiliSeconds.HasValue)
            {
                Trace.WriteLine(string.Format("\tcost: {0}ms", elapsedMiliSeconds));
            }
        }

        public void Log(Stack<AsyncCallContext> callStack)
        {
            if (callStack == null || callStack.Count == 0)
                return;

            Trace.WriteLine("Callgraph Start =====");
            var copy = new Stack<AsyncCallContext>(callStack);
            int indentCount = 1;
            while (copy.Count > 0)
            {
                var callContext = copy.Pop();
                Trace.WriteLine(string.Format("{0}{1}: {2}", ("".PadLeft(indentCount*2)), callContext.MethodName, callContext.InArgs));
                indentCount++;
            }
            Trace.WriteLine("Callgraph End =====");
        }
    }
}
