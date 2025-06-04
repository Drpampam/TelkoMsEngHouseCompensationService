public class UniqueIdGenerator
{
    private static readonly object lockObject = new object();
    private static long lastTimestamp = 0L;
    private static readonly Random random = new Random();

    public static long GenerateUniqueId()
    {
        lock (lockObject)
        {
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (currentTimestamp <= lastTimestamp)
            {
                currentTimestamp = lastTimestamp + 1;
            }

            lastTimestamp = currentTimestamp;
            return ConstructUniqueId(currentTimestamp);
        }
    }

    private static long ConstructUniqueId(long timestamp)
    {
        long prefix = 2000000000000000L;
        long timestampPart = timestamp % 10000000000000L;
        int randomPart = random.Next(100);
        long uniqueId = prefix + timestampPart * 100 + randomPart;
        return uniqueId;
    }

    private string GenerateSequenceNo()
    {
        DateTime dtTm = DateTime.UtcNow;
        DateTime dt = dtTm.Date;

        string yearStr = dtTm.ToString("yy");
        int julianDay = dt.DayOfYear;
        int totalSeconds = dtTm.Hour * 3600 + dtTm.Minute * 60 + dtTm.Second;
        string milliSecondsStr = dtTm.Millisecond.ToString("D6");
        string uniqueIdStr = yearStr + julianDay.ToString("D3") + totalSeconds.ToString("D5") + milliSecondsStr;

        return uniqueIdStr;
    }
}