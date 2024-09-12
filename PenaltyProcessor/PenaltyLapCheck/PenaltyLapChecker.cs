using PenaltyProcessor.Objects;
using RaceroomSharedMemory.R3E;
using static PenaltyProcessor.Objects.TrackVectorCollection;

namespace PenaltyProcessor {
	public class PenaltyLapChecker {

		private TrackVectorCollection myTrackVectors;

		public PenaltyLapChecker(R3EData data) {
			myTrackVectors = new TrackVectorCollection(data);
		}

		public void UpdateTrackInformation(R3EData data) {
			var update = false;
			if (data.MyData.TrackId != this.myTrackVectors.TrackId) {
				update = true;
			}
			if (data.MyData.LayoutId != this.myTrackVectors.LayoutId) {
				update = true;
			}
			if (ConvertTrackNameToString(data.MyData.TrackName) != this.myTrackVectors.TrackName) {
				update = true;
			}
			if (update) {
				myTrackVectors = new TrackVectorCollection(data);
			}
		}

		public bool CurrentVectorIsInPenaltyLap(R3EData data) {
			if (data.MyData.LapDistance < this.myTrackVectors.penaltyLapStart) {
				return false;
			}
			if (data.MyData.LapDistance > this.myTrackVectors.penaltyLapEnd) {
				return false;
			}
			var pointToCheck = new Vector2<double>() {
				X = data.MyData.Player.Position.X,
				Y = data.MyData.Player.Position.Y
			};
			if (!XCoordinateIsInRange(pointToCheck)) {
				return false;
			}
			if (!YCoordinateIsInRange(pointToCheck)) {
				return false;
			}
			return true;
		}

		private bool XCoordinateIsInRange(Vector2<double> pointToCheck) {
			var numerator = (pointToCheck.X - myTrackVectors.LocationVector.X) * myTrackVectors.DirectionalVectorInRaceDirection.X + (pointToCheck.Y - myTrackVectors.LocationVector.Y) * myTrackVectors.DirectionalVectorInRaceDirection.Y;
			var denominator = myTrackVectors.DirectionalVectorLeftToRight.X * myTrackVectors.DirectionalVectorInRaceDirection.X + myTrackVectors.DirectionalVectorLeftToRight.Y * myTrackVectors.DirectionalVectorInRaceDirection.Y;
			var result = numerator / denominator;
			return result > 0 && result <= 1;
		}

		private bool YCoordinateIsInRange(Vector2<double> pointToCheck) {
			var numerator = (pointToCheck.X - myTrackVectors.LocationVector.X) * myTrackVectors.DirectionalVectorLeftToRight.X + (pointToCheck.Y - myTrackVectors.LocationVector.Y) * myTrackVectors.DirectionalVectorLeftToRight.Y;
			var denominator = myTrackVectors.DirectionalVectorLeftToRight.X * myTrackVectors.DirectionalVectorInRaceDirection.X + myTrackVectors.DirectionalVectorLeftToRight.Y * myTrackVectors.DirectionalVectorInRaceDirection.Y;
			var result = numerator / denominator;
			return result > 0 && result <= 1;

		}

	}

}
