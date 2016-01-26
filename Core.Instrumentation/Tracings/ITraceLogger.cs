namespace Core.Instrumentation.Tracings
{
    using System.Collections.Generic;
    using Core.Instrumentation.Tracking;

    public interface ITraceLogger
    {
        void Enter(Categories category, Layers layer, string className, string methodName, string message, string inArgs);
        void Exit(Categories category, Layers layer, string className, string methodName, string message, string outArgs, long? elapsedMiliSeconds);

        void Log(Stack<AsyncCallContext> callStack);
    }
}
