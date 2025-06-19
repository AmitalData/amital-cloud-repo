using System;
using System.Globalization;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class AmitalConvertUtil
    {
        public static DateTime? GetUnifreightFormatedDate(string txt, string dtdField)
        {
            DateTime date;
            if (string.IsNullOrWhiteSpace(txt)) return null;
            if (DateTime.TryParseExact(txt, "dd'.'MM'.'yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            if (DateTime.TryParseExact(txt, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            if (DateTime.TryParseExact(txt, "ddMMyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            if (DateTime.TryParseExact(txt, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            if (DateTime.TryParseExact(txt, "dd'/'MM'/'yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            if (DateTime.TryParseExact(txt, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            if (DateTime.TryParseExact(txt, "yyyyMMddHHmmffff", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            throw new Exception("AmitalConvertUtil:GetShortDate:value=" + txt + " Field=" + dtdField);
        }
    }
}
