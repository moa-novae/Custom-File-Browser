using System;
using NUnit.Framework;
using TechAssessment;

namespace Q2_3Test
{
    [TestFixture]
    public class Q2Tests
    {
        [TestCase(12, 0, 0 )]
        [TestCase(12, 5, 27.5)]
        [TestCase(1, 5, 2.5)]
        [TestCase(5, 30, 15)]
        [TestCase(8, 24, 108)]
        [TestCase("6", "33", 1.5)]
        public void Q2_Always_ReturnExpectedResult(int hour, int min, double expectedResult)
        {
            var result = Q2.FindAngleBetweenClockHands(hour, min);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(24, 10)]
        [TestCase(24, 60)]
        [TestCase(10, 60)]
        public void Q2_ThrowArgumentException_ForInvalidIntArguments(int hour, int min)
        {
            var ex = Assert.Throws<ArgumentException>(() => Q2.FindAngleBetweenClockHands(hour, min));          
        }

        [TestCase("a", "b")]
        [TestCase("2.2", "10")]
        public void Q2_ThrowFormatException_ForInvalidStringArguments(string hour, string min)
        {
            var ex = Assert.Throws<FormatException>(() => Q2.FindAngleBetweenClockHands(hour, min));
        }

        [TestCase("24", "0")]
        [TestCase("20", "60")]
        public void Q2_ThrowArgumentException_ForInvalidStringArguments(string hour, string min)
        {
            var ex = Assert.Throws<ArgumentException>(() => Q2.FindAngleBetweenClockHands(hour, min));
        }
    }
}
