using System;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class StringHelper
    {
        public static string TruncateLongString(string str, int maxLength)
        {
            if (!string.IsNullOrEmpty(str))

                return str.Substring(0, Math.Min(str.Length, maxLength));

            else
                return str;

            //sb.ToString().TrimEnd( '\r', '\n' );
        }

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DateTime? GetDate(string dateString)
        {
            DateTime? myResult = null;

            if (!string.IsNullOrEmpty(dateString))
            {
                string[] dateParts = dateString.Split(':');

                int Year = 0;
                if (!string.IsNullOrEmpty(dateParts[0]))
                {
                    Int32.TryParse(dateParts[0], out Year);
                }

                int Month = 0;
                if (!string.IsNullOrEmpty(dateParts[1]))
                {
                    Int32.TryParse(dateParts[1], out Month);
                }

                int Day = 0;
                if (!string.IsNullOrEmpty(dateParts[2]))
                {
                    Int32.TryParse(dateParts[2], out Day);
                }

                int Hour = 0;
                if (!string.IsNullOrEmpty(dateParts[3]))
                {
                    Int32.TryParse(dateParts[3], out Hour);
                }

                int Minut = 0;
                if (!string.IsNullOrEmpty(dateParts[4]))
                {
                    Int32.TryParse(dateParts[4], out Minut);
                }

                int Second = 0;
                if (!string.IsNullOrEmpty(dateParts[5]))
                {
                    Int32.TryParse(dateParts[5], out Second);
                }

                myResult = new DateTime(Year, Month, Day, Hour, Minut, Second);
            }

            return myResult;
        }


        public static string StringPadRight(string str, char paddingChar, int totalWidth)
        {
            if (!string.IsNullOrEmpty(str))

                return str.PadRight(totalWidth, paddingChar);

            else
                return str;
        }
        public static string StringPadLeft(string str, char paddingChar, int totalWidth)
        {
            if (!string.IsNullOrEmpty(str))

                return str.PadLeft(totalWidth, paddingChar);

            else
                return str;
        }
    }
}
