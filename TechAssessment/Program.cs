using System;


namespace TechAssessment
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Q3.Multiply());
            try
            {
                Console.WriteLine(Q2.FindAngleBetweenClockHands(24, 30));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();

        }

    }
}
