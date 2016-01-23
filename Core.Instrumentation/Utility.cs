namespace Core.Instrumentation
{
	using System;
	using System.Diagnostics;

	class Utility
	{
		public static readonly Stopwatch GlobalStopwatch = Stopwatch.StartNew();

		public static Decimal TicksDiffInMs(long savedTicksCount)
		{
			return TicksDiffInMs(GlobalStopwatch.ElapsedTicks, savedTicksCount);
		}

		public static Decimal TicksDiffInMs(long elapsedTicksCount, long savedTicksCount)
		{
			return TicksDiff(elapsedTicksCount, savedTicksCount) / ((decimal)Stopwatch.Frequency) * 1000;
		}

		public static long TicksDiff(long savedTicksCount)
		{
			return TicksDiff(GlobalStopwatch.ElapsedTicks, savedTicksCount);
		}

		public static long TicksDiff(long elapsedTicksCount, long savedTicksCount)
		{
			long tickDiff = elapsedTicksCount - savedTicksCount;
			if (tickDiff < 0) tickDiff = long.MaxValue + tickDiff; // in the unlikely event the ticks rolled over
			return tickDiff;
		}
	}
}
