namespace AutoBot
{
    public partial class EX9Utils
    {
        private static int GetMonths(int nowMonth, int sDateMonth)
        {
            int currentMonth = nowMonth; // June
            int targetMonth = sDateMonth; // July
            int monthsBetween = 0;

            while (currentMonth != targetMonth)
            {
                currentMonth--;
                if (currentMonth == 0)
                {
                    currentMonth = 12;
                }
                monthsBetween++;
            }
            return monthsBetween;
        }
    }
}