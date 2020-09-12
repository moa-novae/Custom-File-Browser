/* A circle has 360 degrees. Position 12:00 is assumed to be 0 degree.
 * To find angle between clock hands: 
 * 1) find angle between hour hand and position 12:00
 * 2) find angle between minute hand and position 12:00
 * 3) difference between angles obtained from step 1) and step 2) is the angle between clock hands
 *  
 * For every minute passed in 12 hours, the minute hand travels 360 / 60 = 6 degrees
 * the hour hand travels 360 / (60 * 12) = 0.5 degree
 **/
using System;

namespace TechAssessment
{
    class Q2
    {
        /// <summary>
        /// Find the angle between the hour hand and the minute hand on a clock
        /// </summary>
        /// <param name="hour">The hour of the time</param>
        /// <param name="min">The minute of the time</param>
        /// <returns>the smallest angle between the hour hand and minute hand on a clock</returns>
        public static double FindAngleBetweenClockHands(int hour, int min)
        {
            // handle invalid hour or min parameters
            if (hour > 24 || hour < 0)
            {
                throw new ArgumentException(String.Format("{0} hour is not a valid hour", hour), "hour");
            }
            if (min > 60 || min < 0)
            {
                throw new ArgumentException(String.Format("{0} minute is not a valid minute", min), "min");
            }
            int TotalMinutesPassed = hour * 60 + min;
            double MinuteHandDeg = 6 * TotalMinutesPassed % 360;
            double HourHandDeg = 0.5 * TotalMinutesPassed % 360;
            double AngleBetweenHands = Math.Abs(HourHandDeg - MinuteHandDeg);
            // Since angle between hands can be measured clockwise or counter-clockwise, take the smallest of the two
            return Math.Min(360 - AngleBetweenHands, AngleBetweenHands);

        }
    }
}
