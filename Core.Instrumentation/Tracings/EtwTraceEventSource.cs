namespace Core.Instrumentation.Tracings
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Tracing;
    using Core.Instrumentation.Tracking;

    [EventSource(Name = "Test-Instrumentation-ETW")]
	public sealed class EtwTraceEventSource : EventSource, ITraceLogger
    {
		#region log
		private static Lazy<EtwTraceEventSource> _log = new Lazy<EtwTraceEventSource>();

		public static EtwTraceEventSource Log
		{
			get { return _log.Value; }
		}

		public EtwTraceEventSource()
		{
		}
		#endregion

		#region nested
		public class Tasks
		{
			public const EventTask TimedAction = (EventTask)0x1;
			public const EventTask Action = (EventTask)0x2;
		}

		public static class EventIds
		{
			public const int TraceActionTimedStartEventId = 1;
			public const int TimeMethod = 2;
			public const int MethodStart = 3;
			public const int MethodEnd = 4;
			public const int TraceInformationEventId = 5;
			public const int TraceWarningEventId = 6;
			public const int TraceErrorEventId = 7;
			public const int TraceExceptionEventId = 99;
		}
		
		#endregion

		#region log method
		[Event(EventIds.TimeMethod, Message = "elapsed: {0} ms")]
		public void TimeMethod(decimal elapsedMiliSeconds, Categories category, Layers layer)
		{
			if (IsEnabled())
			{
				WriteEvent(EventIds.TimeMethod, elapsedMiliSeconds, category, layer);
			}
		}
        #endregion
        [Event(EventIds.MethodStart, Message = "{0}")]
        public void Enter(Categories category, Layers layer, string className, string methodName, string message, string inArgs)
        {
            if (IsEnabled())
            {
                WriteEvent(EventIds.MethodStart, category, layer, className, methodName, message, inArgs);
            }
        }

        [Event(EventIds.MethodEnd, Message = "{0}")]
        public void Exit(Categories category, Layers layer, string className, string methodName, string message, string outArgs, long? elapsedMiliSeconds)
        {
            if (IsEnabled())
            {
                WriteEvent(EventIds.MethodEnd, category, layer, className, methodName, message, outArgs, elapsedMiliSeconds);
            }
        }

        void ITraceLogger.Log(Stack<AsyncCallContext> callStack)
        {
            // DO nothing
        }
    }
}
