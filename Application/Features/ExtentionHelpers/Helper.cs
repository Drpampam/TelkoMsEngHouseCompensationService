using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;

namespace Application.Features.ExtentionHelpers
{
    public static class Helper
    {

        private static IConfiguration _configuration;
        private static IBulkPostingRepository _bulkPostingRepository;

        static Helper()
        {
            // Set up the configuration
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
        }

        public static void Initialize(IBulkPostingRepository bulkPostingRepository)
        {
            _bulkPostingRepository = bulkPostingRepository;
        }


        public static string GenerateSequuenceNo(this DateTime dateTime)
        {
            long timestamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
            Random random = new Random();
            int randomPart = random.Next(20, 99999);

            string uniqueReference = timestamp.ToString() + randomPart.ToString();

            return uniqueReference;
        }

        private static long ConstructUniqueId(int sequence)
        {
            DateTime dateTime = DateTime.Now;
            var timestamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();

            DateTime baseDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSinceBase = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime - baseDate;

            int year = timeSinceBase.Days / 365 + 2000; // Approximate year
            int dayOfYear = timeSinceBase.Days % 365 + 1; // Approximate day of year
            int secondsOfDay = (int)timeSinceBase.TotalSeconds % 86400;
            int milliseconds = (int)(timestamp % 1000);

            long yearPart = (year % 100) * 100000000000000L;
            long dayPart = dayOfYear * 100000000000L;
            long secondsPart = secondsOfDay * 100000L;
            long millisecondsPart = milliseconds * 100L;
            long sequencePart = sequence;

            return yearPart + dayPart + secondsPart + millisecondsPart + sequencePart;
        }

        public static string ExtractFileName(string fullPath)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
                return fileNameWithoutExtension != null ? fileNameWithoutExtension.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                return new Exception().Message.ToString() ?? ex.ToString();
            }
        }

        public static string GenerateUniqueReference(this DateTime dateTime)
        {
            var timestamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
            var random = new Random();
            var randomPart = random.Next(10000000, 99999999);

            var uniqueReference = timestamp + randomPart.ToString();

            return uniqueReference;
        }

        public static string GenerateUniqueReferenceV2(this DateTime dateTime)
        {
            var timestamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
            var random = new Random();
            var randomPart = random.Next(10, 99999999);

            var uniqueReference = timestamp + randomPart.ToString();

            return uniqueReference + ".csv";
        }

        public static DateTime? ConvertToDateTime(string inputTime)
        {
            DateTime targetTime;

            if (DateTime.TryParseExact(inputTime, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out targetTime)) return targetTime;

            if (DateTime.TryParse(inputTime, out targetTime)) return targetTime.ToLocalTime();
            return null;
        }

        public static DateTime? ConvertToDateTimeV2(string inputTime)
        {
            DateTime targetTime;

            if (DateTime.TryParseExact(inputTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out targetTime)) return targetTime;
            return null;
        }

        public static string ConvertFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        private static int GenerateNumericValue()
        {
            Random random = new Random();
            return random.Next(10000, 10000000);
        }
    }
}
