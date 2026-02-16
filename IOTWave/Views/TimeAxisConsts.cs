using System;

namespace IotWave.Views;

public class TimeAxisConsts
{
    public static readonly long[] TimeIntervals = {
        TimeSpan.TicksPerMillisecond / 1000,    // 100 nanoseconds
        TimeSpan.TicksPerMillisecond / 100,     // 1 microsecond
        TimeSpan.TicksPerMillisecond / 10,      // 10 microseconds
        TimeSpan.TicksPerMillisecond,           // 1 millisecond
        TimeSpan.TicksPerMillisecond * 10,      // 10 milliseconds
        TimeSpan.TicksPerMillisecond * 50,      // 50 milliseconds
        TimeSpan.TicksPerMillisecond * 100,     // 100 milliseconds
        TimeSpan.TicksPerSecond,                // 1 second
        TimeSpan.TicksPerSecond * 2,            // 2 seconds
        TimeSpan.TicksPerSecond * 5,            // 5 seconds
        TimeSpan.TicksPerSecond * 10,           // 10 seconds
        TimeSpan.TicksPerSecond * 15,           // 15 seconds
        TimeSpan.TicksPerSecond * 30,           // 30 seconds
        TimeSpan.TicksPerMinute,                // 1 minute
        TimeSpan.TicksPerMinute * 2,            // 2 minutes
        TimeSpan.TicksPerMinute * 5,            // 5 minutes
        TimeSpan.TicksPerMinute * 10,           // 10 minutes
        TimeSpan.TicksPerMinute * 15,           // 15 minutes
        TimeSpan.TicksPerMinute * 30,           // 30 minutes
        TimeSpan.TicksPerHour,                  // 1 hour
        TimeSpan.TicksPerHour * 2,              // 2 hours
        TimeSpan.TicksPerHour * 4,              // 4 hours
        TimeSpan.TicksPerHour * 6,              // 6 hours
        TimeSpan.TicksPerHour * 12,             // 12 hours
        TimeSpan.TicksPerDay                    // 1 day
    };
}