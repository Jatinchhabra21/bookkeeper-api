namespace BookkeeperAPI.Service
{
    #region usings
    using System;
    using System.Text;
    #endregion
    public static class OneTimePassword
    {
        public static string Generate()
        {
            long ticks = DateTime.UtcNow.Ticks;
            string totalSeconds = Convert.ToString(TimeSpan.FromTicks(ticks).TotalSeconds);
            StringBuilder stringBuilder = new StringBuilder();
            int index = 0;

            while (stringBuilder.Length < 6) 
            {
                if (totalSeconds[totalSeconds.Length - index - 1] - 48 > 0)
                    stringBuilder.Append(totalSeconds[totalSeconds.Length - index - 1] - 48);
                index++;
            }

            string otp = stringBuilder.ToString();

            do
            {
                Random random = new Random();
                int numberOfRandomNumbersInOTP = random.Next(1, 4);
                for (int i = 1; i <= numberOfRandomNumbersInOTP; i++)
                {
                    Random rand = new Random();
                    int randomIndex = rand.Next(0, 6);
                    StringBuilder otpStr = new StringBuilder(otp);
                    otpStr[randomIndex] = random.Next(0, 10).ToString()[0];
                    otp = otpStr.ToString();
                }
            } while (false);

            return otp.Length == 6 ? otp : otp.PadRight(6, '0');
        }
    }
}
