using System.Globalization;
using System.Net.Sockets;

namespace ArtGCS;

public static class Time
{
    public static DateTime GetCurrentDateTime()
    {
        // var client = new TcpClient("time.nist.gov", 13);
        // using var streamReader = new StreamReader(client.GetStream());
        // var response = streamReader.ReadToEnd();
        // if (response.Length < 1)
        //     return DateTime.UtcNow;
        //
        // var utcDateTimeString = response.Substring(7, 17);
        //
        // return DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        return DateTime.UtcNow;
    }

    public static TimeOnly GetCurrentTimeOnly()
    {
        return TimeOnly.FromDateTime(GetCurrentDateTime());
    }
}