using NUnit.Framework;
using SlugEnt;
using System;

namespace Test_DayTimeInterval
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("6:30:02",23402)]
        [TestCase("6:30:02 PM", 66602)]
        [TestCase("18:30:02 AM", 66602)]
        [TestCase("6:30", 23400)]
        [TestCase("6", 21600)]
        [TestCase("6:30", 23400)]
        [TestCase("6:30:02AM",23402)]
        [TestCase("6:30:02 AM", 23402)]
        [TestCase("6:30:02 AM ", 23402)]
        [TestCase("6:30:02 AM ", 23402)]
        [TestCase("06:30:02AM", 23402)]
        [TestCase("06:30:02 AM ", 23402)]
        public void TimeInterval_Success(string time, int seconds)
        {         
            Assert.AreEqual(seconds * DayTimeInterval.TICKS_IN_SECOND, DayTimeInterval.ConvertTimeString (time));
        }


        [Test]
        [TestCase("25:00:4")]
        [TestCase("15:61:4")]
        [TestCase("15:11:74")]
        [TestCase("15s")]
        [TestCase("sec")]
        [TestCase("3:sec")]
        [TestCase("3:30:sec")]
        [TestCase("3:30fm")]
        [TestCase("3:30 fm")]
        [TestCase("3:30 america")]
        [TestCase("3:30 iam")]
        public void TimeInterval_Throws_Success (string time)
        {
            Assert.Throws<ArgumentException>(() => DayTimeInterval.ConvertTimeString(time));
        }


        [Test]
        [TestCase(4,0,0,true)]
        [TestCase(13, 15, 0, true)]
        [TestCase(15, 0, 0, true)]
        [TestCase(3, 15, 0, false)]
        [TestCase(16, 15, 0, false)]
        [TestCase(23, 59, 59, false)]
        [TestCase(0, 0, 0, false)]
        public void IsInIntervalNormal (int hour, int minute, int second,bool result)
        {

            DateTime current = DateTime.Now;
            DateTime simulated = new DateTime(current.Year, current.Month, current.Day,hour,minute,second);


            // Create the interval
            DayTimeInterval timeInterval = new DayTimeInterval("4am", "3pm");
            Assert.AreEqual(result,timeInterval.IsInInterval(simulated), "A10: ");
        }




        [Test]
        [TestCase(4, 0, 0, false)]
        [TestCase(13, 15, 0, false)]
        [TestCase(15, 0, 0, false)]
        [TestCase(3, 0, 0, true)]
        [TestCase(18, 15, 0, true)]
        [TestCase(23, 59, 59, true)]
        [TestCase(0, 0, 0, true)]
        public void IsInIntervalNegativeTime(int hour, int minute, int second, bool result)
        {

            DateTime current = DateTime.Now;
            DateTime simulated = new DateTime(current.Year, current.Month, current.Day, hour, minute, second);


            // Create the interval
            DayTimeInterval timeInterval = new DayTimeInterval("6pm", "3am");
            Assert.AreEqual(result, timeInterval.IsInInterval(simulated), "A10: ");
        }
    }
}