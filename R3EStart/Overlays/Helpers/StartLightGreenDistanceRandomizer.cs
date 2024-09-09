using System;

namespace RaceroomCompanion.Overlays.Helpers {
	
	public class StartLightGreenDistanceRandomizer {

		public int SpeedLimitDistance { get; private set; }
		public int RedLightDistance { get; private set; }
		public int GreenLightDistance { get; private set; }

		public StartLightGreenDistanceRandomizer() : this(1200, 400) {

		}

		public StartLightGreenDistanceRandomizer(int speedLimitDistance, int redLightDistance) {
			int seed = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
			Random random = new Random(seed);
			this.GreenLightDistance = new Random(seed).Next(5, 120);
			this.RedLightDistance = redLightDistance;
			this.SpeedLimitDistance = speedLimitDistance;
		}
	
	}

}
