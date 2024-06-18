using System;

namespace R3EStart.Helpers {
	public static class Constants {
		public const string R3EName32 = "RRRE";
		public const string R3EName64 = "RRRE64";
		public const string RRREWebBrowserName = "RRREWebBrowser";
		public const string ReStartName = "R3EStart";
		public const string ReStartExeName = "R3EStart.exe";
	}

	public class OverlayDistances {
		public int SpeedLimitDistance;
		public int RedLightDistance;
		public int GreenLightDistance;
		public OverlayDistances() : this(1200, 400) {

		}
		public OverlayDistances(int speedLimitDistance, int redLightDistance) {
			int seed = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
			Random random = new Random(seed);
			this.GreenLightDistance = new Random(seed).Next(5, 120);
			this.RedLightDistance = redLightDistance;
			this.SpeedLimitDistance = speedLimitDistance;
		}
	}
}
