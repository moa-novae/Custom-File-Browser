using TechAssessment;
using NUnit.Framework;
using System;

namespace Q2_3Test
{
    [TestFixture]
    class Q3Tests
    {
        [TestCase(2, 3, 6)]
        [TestCase(2.5, 2.0, 5)]
        [TestCase(0, 0, 0)]
        [TestCase(-5, 2, -10)]
        [TestCase(-5, -2, 10)]

        public void Q3_Always_ReturnExpectedResult_WithDoubleArgs(double m1, double m2, double expectedResult)
        {
            var result = Q3.Multiply(m1, m2);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(0, 0, 0)]
        [TestCase(2, 3, 6)]
        [TestCase(-5, 2, -10)]
        public void Q3_Always_ReturnExpectedResult_WithIntArgs(int m1, int m2, double expectedResult)
        {
            var result = Q3.Multiply(m1, m2);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("-5", "2.2", -11)]
        [TestCase("-5.0", "-2.2", 11)]
        [TestCase("0", "0", 0)]
        public void Q3_Always_ReturnExpectedResult_WithStringArgs(string m1, string m2, double expectedResult)
        {
            var result = Q3.Multiply(m1, m2);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("a", "2.2")]
        [TestCase("-5.0", "@")]
        public void Q3_ThrowFormatException_ForInvalidStringArguments(string m1, string m2)
        {
            var ex = Assert.Throws<FormatException>(() => Q2.FindAngleBetweenClockHands(m1, m2));
        }
    }
}
