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
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
      return dateTime;
    }
  }
}
