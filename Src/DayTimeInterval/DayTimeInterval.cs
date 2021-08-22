using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo ("Test_DayTimeInterval")]
namespace SlugEnt
{
    
    public class DayTimeInterval
    {
        public const long TICKS_IN_SECOND = 10000000;

        /// <summary>
        /// The number of ticks since midnight that the interval starts at.
        /// </summary>
        public long StartTime { get; set; } = -1;
        
        
        /// <summary>
        /// The number of ticks since midnight that the interval stops at.
        /// </summary>
        public long EndTime { get; set; } = -1;


        /// <summary>
        /// If true, then the interval to check is midnight to start AND end to midnight.  If false, it is start to end.
        /// </summary>
        private bool IsNegativeCheck { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startTime">Time in format of 12:30:02 AM, or 4pm,  or 16:20</param>
        /// <param name="endTime">Time in format of 12:30:02 AM, or 4pm,  or 16:20</param>
        public DayTimeInterval (string startTime, string endTime) { 
            StartTime = ConvertTimeString(startTime);
            EndTime = ConvertTimeString(endTime);
            DetermineIntervalType(StartTime, EndTime);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startTime">Ticks since midnight of the start time of the interval</param>
        /// <param name="endTime">Ticks since midnight of the end time of the interval</param>
        public DayTimeInterval (long startTime, long endTime) {
            StartTime = startTime;
            EndTime = endTime;
            DetermineIntervalType(StartTime, EndTime);
        }


        /// <summary>
        /// This determines whether we need to compare between start and end OR between midnight and start AND end and Midnight
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private void DetermineIntervalType (long startTime, long endTime)
        {
            if (endTime < startTime) { IsNegativeCheck = true; }
            else IsNegativeCheck = false;   
        }



        /// <summary>
        /// Returns true if the time portion of the given DateTime parameter is within the interval.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool IsInInterval (DateTime dateTime) 
        {
            long currentTicks = dateTime.Ticks;
            


            // Get Millisecs since midnight
            DateTime midnight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            
            if (IsNegativeCheck) {
                if ((midnight.Ticks + StartTime) <= currentTicks) return true;
                if ((midnight.Ticks + EndTime) >= currentTicks) return true;
                return false;
            }

            if ((midnight.Ticks + StartTime) <= currentTicks && (midnight.Ticks + EndTime) >= currentTicks) return true;
            return false;
        }


        /// <summary>
        /// Takes a time string and converts it to Ticks since midnight.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertTimeString (string time)
        {
            if (time == null || time == string.Empty) { throw new ArgumentException("Time value string must be supplied"); }

            string[] parts = time.Split (':');
            int hour = 0;
            int minute = 0;
            int second = 0;
            int millisecond = 0;

            bool isAM = true;
            bool lastPartWasAMPM = false;

            // Determine what the last part is, then we know what the others should be.
            string lastPart = parts[parts.Length - 1].ToLower().Trim();

            if (!int.TryParse(lastPart, out _))
            {
                if (lastPart.EndsWith("am"))
                {
                    isAM = true;
                    lastPart = lastPart.Remove(lastPart.IndexOf("am"));
                    parts[parts.Length - 1] = lastPart;
                }
                else if (lastPart.EndsWith("pm"))
                {
                    isAM = false;
                    lastPart = lastPart.Remove(lastPart.IndexOf("pm"));
                    parts[parts.Length - 1] = lastPart;
                }
                else
                    throw new ArgumentException("Invalid time string.  Contains trailing characters that are not numeric or AM/PM - [" + lastPart + "]");

                if (lastPart == string.Empty) lastPartWasAMPM = true;
            }


            // If you have AM/PM then there has to be more than just 1 part - ie, you need 1pm or 2pm...
            if (lastPartWasAMPM && parts.Length == 1) { throw new ArgumentException("Invalid time string: no numeric time identifier - [" + time +"]"); }


            // Ok, past the hard part, now we just need to convert the remaining parts into time parts, based upon their numeric values
            // The first value must be hour, second value the minute and the third the seconds
            int length = lastPartWasAMPM ? parts.Length - 1 : parts.Length;
            //int length = lastPart.Length;

            if (length > 2)
            {
                if (!int.TryParse(parts[2], out second)) { throw new ArgumentException("Invalid time string - [" + time + "]"); }
            }

            if (length > 1)
            {
                if (!int.TryParse(parts[1], out minute)) { throw new ArgumentException("Invalid time string - [" + time + "]"); }
            }

            if (!int.TryParse(parts[0], out hour)) { throw new ArgumentException("Invalid time string - [" + time + "]"); }


            // Validate the time values
            if (!isAM && hour < 12) hour += 12;
            if (hour > 24 ) throw new ArgumentException("Invalid hour argument for the time [" + hour + "]");
            if (minute > 60 ) throw new ArgumentException("Invalid minute argument for the time [" + minute + "]");
            if (second > 60 )   throw new ArgumentException("Invalid secoond argument for the time [" + second + "]");

            // Now convert to milliseconds4
            //int seconds = (second + minute * 60 + hour * 3600);

            long ticks = TICKS_IN_SECOND * (second + minute * 60 + hour * 3600);
            return ticks;
        }

       
    }
}
