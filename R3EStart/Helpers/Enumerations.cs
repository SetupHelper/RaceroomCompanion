namespace ReStart.Helpers
{
    public enum CarOperatingStatus
    {
        InMenu = -1,
        OnTrack = 0,
        InPits = 1,
    }

    public enum Session
    {
        Unavailable = -1,
        Practice = 0,
        Qualify = 1,
        Race = 2,
        Warmup = 3,
        TestDay = 4,
    }

    public static class Constants
    {
        public const string R3EName32 = "RRRE";
        public const string R3EName64 = "RRRE64";
        public const string RRREWebBrowserName = "RRREWebBrowser";
        public const string ReStartName = "R3EStart";
        public const string ReStartExeName = "R3EStart.exe";
    }

}
