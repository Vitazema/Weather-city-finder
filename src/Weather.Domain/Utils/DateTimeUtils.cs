using System;

namespace Weather.Domain.Utils
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// Конвертирует универсальный формат unix времени в DateTime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return DateTime.UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}