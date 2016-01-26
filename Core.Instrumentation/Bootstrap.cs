namespace Core.Instrumentation
{
    using System.Configuration;
    using Core.Instrumentation.Tracings;

    public class Bootstrap
    {
        private static ITraceLogger logger;

        public static ITraceLogger GetLogger()
        {
            if (logger == null)
            {
                if (ConfigurationManager.AppSettings.Get("TraceLoggerType") == "Mock")
                {
                    logger = new ConsoleTraceLogger();
                }
                else
                {
                    logger=new EtwTraceEventSource();
                }
            }
            return logger;
        }
    }
}
