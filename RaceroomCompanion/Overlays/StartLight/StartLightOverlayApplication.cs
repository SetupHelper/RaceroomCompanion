using System;
using System.Linq;
using System.Threading;
using log4net;
using RaceroomCompanion.Overlays.Helpers;
using RaceroomCompanion.Overlays.OverlayView;
using RaceroomSharedMemory.R3E;
using RaceroomSharedMemory.R3E.Data;

namespace RaceroomCompanion.Overlays.StartLight {

	public class StartLightOverlayApplication : RaceroomCompanionOverlayApplication {

		private readonly R3EData R3EData;
		internal int AllowedStartSpeed = 0;
		private DriverData PoleSetter => R3EData.MyData.DriverData.FirstOrDefault();
		private static readonly ILog log = LogManager.GetLogger("RollingFileAppender");
		public StartLightGreenDistanceRandomizer distanceRandomizer;
		private StartLightOverlayView myStartLightView;

		public StartLightOverlayApplication(string processName) : base(processName) {
			log.Info("Starting R3EStart overlay");
			R3EData = Utilities.MapData();
			distanceRandomizer = new StartLightGreenDistanceRandomizer();
		}

		internal void Start() {
			myStartLightView = new StartLightOverlayView(this);
		}

		internal void UpdateData() {
			this.R3EData.Read();
			SetSpeedLimitForCurrentTrack();
		}

		internal void SetSpeedLimitForCurrentTrack() {
			var speedLimit = Math.Round(this.GetSpeedInKmh(this.R3EData.MyData.SessionPitSpeedLimit));
			if (int.TryParse(speedLimit.ToString(), out AllowedStartSpeed)) {
				log.Info($"Parsing allowed start speed:{AllowedStartSpeed} ");
			} else {
				log.Error($"Parsing allowed Start Speed not possible, value {R3EData.MyData.SessionPitSpeedLimit}");
			}
		}

		internal bool RenderSafetyCarSign() {
			if (this.GetRemainingLapDistance() <= distanceRandomizer.SpeedLimitDistance) {
				return false;
			}
			return true;
		}

		internal bool RenderSpeedLimiter() {
			if (this.GetRemainingLapDistance() > distanceRandomizer.SpeedLimitDistance) {
				return false;
			}
			if (this.GetRemainingLapDistance() <= distanceRandomizer.RedLightDistance) {
				return false;
			}
			return true;
		}

		internal bool RenderRedLight() {
			if (this.GetRemainingLapDistance() > distanceRandomizer.RedLightDistance) {
				return false;
			}
			if (this.GetRemainingLapDistance() <= distanceRandomizer.GreenLightDistance) {
				return false;
			}
			return true;
		}

		internal bool PauseForGreenLight { get; private set; }
		internal bool RenderGreenLight() {
			if (this.GetRemainingLapDistance() > distanceRandomizer.GreenLightDistance) {
				return false;
			}
			PauseForGreenLight = true;
			return true;
		}

		internal bool DisplayOverlays() {
			if (R3EData.MyData.SessionType != (int)Session.Race) {
				return false;
			}
			if (R3EData.MyData.GameInMenus == 1) {
				return false;
			}
			if (R3EData.MyData.GamePaused == 1) {
				return false;
			}
			if (PoleSetter.CompletedLaps >= 1) {
				return false;
			}
			if (R3EData.MyData.StartLights < 6) {
				return false;
			}
			if (R3EData.MyData.SessionPhase != 5) {
				return false;
			}
			return true;
		}

		internal float GetSpeedPercentage() {
			var speedInKmh = this.GetSpeedInKmh(this.R3EData.MyData.CarSpeed);
			var percentage = 100 / (float)this.AllowedStartSpeed * speedInKmh;
			return Math.Min(percentage, 100);
		}

		internal float GetSpeedInKmh(float speed) {
			var speedInKmh = speed * 3.6f;
			return speedInKmh;
		}

		internal float GetRemainingLapDistance() {
			var distanceDriven = PoleSetter.LapDistance;
			var totalLapDistance = R3EData.MyData.LayoutLength;
			return totalLapDistance - distanceDriven;
		}

		internal int GetCarSpeed() {
			var speedInKmh = this.GetSpeedInKmh(this.R3EData.MyData.CarSpeed);
			return (int)Math.Round((double)speedInKmh, 0);
		}

		internal void StopPauseForGreenLight() {
			this.PauseForGreenLight = false;
		}

	}
}