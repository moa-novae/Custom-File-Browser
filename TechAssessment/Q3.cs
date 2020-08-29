/* Multiplication can be thought of as a repeated addition
 * 4 * 3 = 4 + 4 + 4
 * We can implement this in recursion to avoid using loops since we are repeatedly adding the multiplier 
 * Multiply(4, 3) = 4 + Multiply(4, 2) = 4 + 4 + Multiply(4, 1) = 4 + 4 + 4 + Multiply(4, 0)
*/
namespace TechAssessment
{
    class Q3
    {
        public static double Multiply(double multiplier = 1, double multiplicand = 1)
        {
            // Base case is achieved when multiplicand is 0
            if (multiplicand == 0)
            {
                return 0;
            }
            // Add multiplier to total while decreasing multiplicand by 1
            else if (multiplicand >= 1)
            {
                return (multiplier + Multiply(multiplier, multiplicand - 1));
            }
            // Handle case when multiplicand has decimal
            else if (multiplicand < 1 && multiplicand > 0)
            {
                return (multiplier * multiplicand + Multiply(multiplier, 0));
            }
            // Handle case when multiplicand is negative and cannot be decremented to 0
            // 4 * -3 = -4 * 3
            if (multiplicand < 0)
            {
                return -Multiply(multiplier, -multiplicand);
            }
            else
            {
                return -1;
            }
        }
    }
}
