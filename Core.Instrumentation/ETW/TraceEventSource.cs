namespace Core.Instrumentation.ETW
{
	using System;
	using System.Diagnostics.Tracing;
	using Core.Instrumentation.Tracking;

	[EventSource(Name = "Test-Instrumentation-ETW")]
	public sealed class TraceEventSource : EventSource
	{
		#region log
		private static Lazy<TraceEventSource> _log = new Lazy<TraceEventSource>();

		public static TraceEventSource Log
		{
			get { return _log.Value; }
		}

		private TraceEventSource()
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
		[Event(EventIds.MethodStart, Message = "{0}")]
		public void BeforeMethod(string enteringMessage, Categories category, Layers layer)
		{
			if (IsEnabled())
			{
				WriteEvent(EventIds.MethodStart, category, layer);
			}
		}

		[Event(EventIds.MethodEnd, Message = "{0}")]
		public void AfterMethod(string enteringMessage, Categories category, Layers layer)
		{
			if (IsEnabled())
			{
				WriteEvent(EventIds.MethodEnd, category, layer);
			}
		}

		[Event(EventIds.TimeMethod, Message = "elapsed: {0} ms")]
		public void TimeMethod(decimal elapsedMiliSeconds, Categories category, Layers layer)
		{
			if (IsEnabled())
			{
				WriteEvent(EventIds.TimeMethod, elapsedMiliSeconds, category, layer);
			}
		}
		#endregion

	}
}
