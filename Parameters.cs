namespace BlueGoat.MongoDBUtils
{
    public static class Parameters
    {
        public static long LargeFileSizeWarningThresholdBytes => LargeFileSizeWarningThresholdInMegaBytes * 1000000;
        public static long LargeFileSizeWarningThresholdInMegaBytes = 4;
    }
}
