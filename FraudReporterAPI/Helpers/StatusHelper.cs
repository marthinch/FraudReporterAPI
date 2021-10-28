namespace FraudReporterAPI.Helpers
{
    public static class StatusHelper
    {
        public static string ConvertStatus(int status)
        {
            string convertedStatus = string.Empty;

            switch (status)
            {
                case 0:
                    convertedStatus = "Not Reported";
                    break;

                case 1:
                    convertedStatus = "Pending Reported";
                    break;

                case 2:
                    convertedStatus = "Reported";
                    break;

                case 3:
                    convertedStatus = "Cancel";
                    break;
            }

            return convertedStatus;
        }
    }
}
