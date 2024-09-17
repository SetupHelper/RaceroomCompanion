using PenaltyProcessor;
using RaceroomSharedMemory.R3E;
using System.Windows;

namespace RaceroomCompanionZoneWriter {

	public partial class MainWindow : Window {

		public bool Recording;
		internal PenaltyLapChecker lapChecker;
		internal readonly R3EData R3EData;
		internal ZoneType SelectedZone;
		internal readonly CancellationTokenSource MyCancellationTokenSource;
		internal CancellationToken CancelToken => MyCancellationTokenSource.Token;



		public MainWindow() {
			InitializeComponent();
			R3EData = Utilities.MapData();
			MyCancellationTokenSource = new CancellationTokenSource();
			R3EData.Read();
			this.lapChecker = new PenaltyLapChecker(this.R3EData);
		}

		private void RecordButtonClicked(object sender, RoutedEventArgs e) {
			if (Recording) {
				Recording = false;
			} else {
				Recording = true;
				Task.Run(RecordCoordinates);
			}
		}

		private void RecordCoordinates() {
			this.R3EData.Read();
			var lastDistance = this.R3EData.MyData.LapDistance;
			while (Recording) {
				this.R3EData.Read();
				if (lastDistance + 1 <= this.R3EData.MyData.LapDistance) {
					lastDistance = this.R3EData.MyData.LapDistance;
					switch (this.SelectedZone) {
						case ZoneType.LeftStartLeft:
							this.lapChecker.myTrackVectors.LeftStartLane.RecordLeftCoordinates(this.R3EData);
							break;
						case ZoneType.LeftStartRight:
							this.lapChecker.myTrackVectors.LeftStartLane.RecordRightCoordinates(this.R3EData);
							break;
						case ZoneType.RightStartLeft:
							this.lapChecker.myTrackVectors.RightStartLane.RecordLeftCoordinates(this.R3EData);
							break;
						case ZoneType.RightStartRight:
							this.lapChecker.myTrackVectors.RightStartLane.RecordRightCoordinates(this.R3EData);
							break;
						case ZoneType.PenaltyLeft:
							this.lapChecker.myTrackVectors.PenaltyLane.RecordLeftCoordinates(this.R3EData);
							break;
						case ZoneType.PenaltyRight:
							this.lapChecker.myTrackVectors.PenaltyLane.RecordRightCoordinates(this.R3EData);
							break;
						default:
							break;
					}
				}
				Thread.Sleep(100);
			}
		}

		private void SaveButtonClicked(object sender, RoutedEventArgs e) {
			this.lapChecker.myTrackVectors.WriteDataToConfigFile();
		}

		public enum ZoneType {
			LeftStartLeft,
			LeftStartRight,
			RightStartLeft,
			RightStartRight,
			PenaltyLeft,
			PenaltyRight,
		}

		private void LeftLaneLeft_Checked(object sender, RoutedEventArgs e) {
			this.SelectedZone = ZoneType.LeftStartLeft;
		}

		private void LeftLaneRight_Checked(object sender, RoutedEventArgs e) {
			this.SelectedZone = ZoneType.LeftStartRight;
		}

		private void RightLaneLeft_Checked(object sender, RoutedEventArgs e) {
			this.SelectedZone = ZoneType.RightStartLeft;
		}

		private void RightLaneRight_Checked(object sender, RoutedEventArgs e) {
			this.SelectedZone = ZoneType.RightStartRight;
		}

		private void PenaltyZoneLeft_Checked(object sender, RoutedEventArgs e) {
			this.SelectedZone = ZoneType.PenaltyLeft;
		}

		private void PenaltyZoneRight_Checked(object sender, RoutedEventArgs e) {
			this.SelectedZone = ZoneType.PenaltyRight;
		}

	}

}