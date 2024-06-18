using System;
using System.Linq;
using System.Threading;
using log4net;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;
using R3EStart.Helpers;
using ReStart.R3E;
using ReStart.R3E.Data;

namespace R3EStart.Overlays.StartLight {

	public class StartLightOverlay : DirectXOverlayPlugin {
		private readonly TickEngine _tickEngine = new TickEngine();
		private int largeFont;
		private int smallFont;
		private int redBrush;
		private int yellowBrush;
		private int greenBrush;
		private int blackBrush;
		private int whiteBrush;
		private int fps = 90;
		public int UpdateRate => 1000 / fps;
		private R3EData R3EData;
		private int allowedStartSpeed = 0;
		private int yCorrection => this.OverlayWindow.Height / 4;
		private int WidthDependingOnScreenHeight => this.OverlayWindow.Height / 8;
		private DriverData PoleSetter => R3EData.myData.DriverData.FirstOrDefault();
		private static readonly ILog log = LogManager.GetLogger("RollingFileAppender");
		public OverlayDistances overlayDistances;

		public override void Initialize(IWindow targetWindow) {
			log.Info("Starting R3EStart overlay");
			base.Initialize(targetWindow);
			R3EData = Utilities.MapData();
			overlayDistances = new OverlayDistances();
			log.Info($"greenLightDistance: {overlayDistances.GreenLightDistance}");
			OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
			var transparency = 160;
			greenBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 0, 100, 0));
			yellowBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 204, 204, 0));
			redBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 204, 0, 0));
			blackBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, System.Drawing.Color.Black));
			whiteBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, System.Drawing.Color.White));
			largeFont = OverlayWindow.Graphics.CreateFont("Arial", 55);
			smallFont = OverlayWindow.Graphics.CreateFont("Arial", 40);
			_tickEngine.PreTick += OnPreTick;
			_tickEngine.Tick += OnTick;
		}

		private void OnTick(object sender, EventArgs e) {
			try {
				if (!OverlayWindow.IsVisible) {
					return;
				}
				R3EData.Read();
				if (allowedStartSpeed <= 0) {
					var speedLimit = Math.Round(GetSpeedInKmh(R3EData.myData.SessionPitSpeedLimit));
					if (int.TryParse(speedLimit.ToString(), out allowedStartSpeed)) {
						log.Info($"Parsing allowed start speed:{allowedStartSpeed} ");
					} else {
						log.Error($"Parsing allowed Start Speed not possible, value {R3EData.myData.SessionPitSpeedLimit}");
					}
				}
				OverlayWindow.Update();
				InternalRender();
			} catch (Exception ex) {
				log.Error(ex.Message);
			}
		}

		private void OnPreTick(object sender, EventArgs e) {
			var targetWindowIsActivated = TargetWindow.IsActivated;
			if (!targetWindowIsActivated && OverlayWindow.IsVisible) {
				ClearScreen();
				OverlayWindow.Hide();
			} else if (targetWindowIsActivated && !OverlayWindow.IsVisible) {
				OverlayWindow.Show();
			}
		}

		public override void Enable() {
			_tickEngine.Interval = UpdateRate.Milliseconds();
			_tickEngine.IsTicking = true;
			base.Enable();
		}

		public override void Disable() {
			_tickEngine.IsTicking = false;
			base.Disable();
		}

		public override void Update() => _tickEngine.Pulse();

		protected async void InternalRender() {
			bool greenlight = false;
			OverlayWindow.Graphics.BeginScene();
			OverlayWindow.Graphics.ClearScene();
			var remainingLapDistance = ComputeRemainingLapDistance();
			if (DisplayOverlays()) {
				if (remainingLapDistance > overlayDistances.SpeedLimitDistance) {
					DisplaySafetyCarSign();
				} else if (remainingLapDistance <= overlayDistances.SpeedLimitDistance && remainingLapDistance > overlayDistances.RedLightDistance) {
					DisplaySpeedLimiter();
				} else if (remainingLapDistance <= overlayDistances.RedLightDistance && remainingLapDistance > overlayDistances.GreenLightDistance) {
					DisplayRedLight();
				} else if (remainingLapDistance <= overlayDistances.GreenLightDistance) {
					greenlight = true;
					_tickEngine.Stop();
					DisplayGreenLight();
					OverlayWindow.Graphics.EndScene();
					Thread.Sleep(4000);
				}
			}
			if (!greenlight) {
				OverlayWindow.Graphics.EndScene();
			}
		}

		private bool DisplayOverlays() {
			if (R3EData.myData.SessionType != (int)Session.Race)
				return false;
			if (R3EData.myData.GameInMenus == 1)
				return false;
			if (R3EData.myData.GamePaused == 1)
				return false;
			if (PoleSetter.CompletedLaps >= 1)
				return false;
			if (R3EData.myData.StartLights < 6)
				return false;
			if (R3EData.myData.SessionPhase != 5)
				return false;
			return true;
		}

		private void DisplaySafetyCarSign() {
			int centerX = this.TargetWindow.Width / 2;
			int centerY = this.TargetWindow.Height / 2 - yCorrection;
			OverlayWindow.Graphics.BorderedRectangle(centerX, centerY, WidthDependingOnScreenHeight, WidthDependingOnScreenHeight, 2, 10, blackBrush, yellowBrush);
		}

		private void DisplaySpeedLimiter() {
			DrawSpeedLimitSign();
			DrawSpeedBar();
		}

		private void DrawSpeedLimitSign() {
			int centerX = this.TargetWindow.Width / 2;
			int centerY = this.TargetWindow.Height / 2 - yCorrection;
			int outerRadius = 44;
			int innerRadius = 40;
			OverlayWindow.Graphics.FillCircle(centerX, centerY, outerRadius, whiteBrush);
			OverlayWindow.Graphics.DrawCircle(centerX, centerY, outerRadius, 10, redBrush);
			OverlayWindow.Graphics.FillCircle(centerX, centerY, innerRadius, whiteBrush);
			OverlayWindow.Graphics.DrawText(allowedStartSpeed.ToString(), largeFont, blackBrush, centerX - 32, centerY - 32);
		}

		private void DrawSpeedBar() {
			int barWidth = 600;
			int centerX = this.TargetWindow.Width / 2 - barWidth / 2;
			int centerY = this.TargetWindow.Height / 2 - yCorrection + 70;
			var barColor = greenBrush;
			var speedInKmh = GetSpeedInKmh(R3EData.myData.CarSpeed);
			if (speedInKmh > allowedStartSpeed + 4) {
				barColor = yellowBrush;
			}
			if (speedInKmh > allowedStartSpeed + 6) {
				barColor = redBrush;
			}
			OverlayWindow.Graphics.DrawBarV(centerX, centerY, 600, 50, Math.Min(GetSpeedPercentage(), 100), 1, blackBrush, barColor);
			OverlayWindow.Graphics.DrawText($"{(int)speedInKmh} km/h", smallFont, blackBrush, centerX, centerY);
		}

		private void DisplayRedLight() {
			DrawStartLight(redBrush);
			log.Info("Displaying red Light");
		}

		private void DrawStartLight(int brushColor) {
			var x = this.TargetWindow.Width / 2 - 190;
			var y = this.TargetWindow.Height / 2 - yCorrection;
			for (int i = 0; i < 5; i++) {
				OverlayWindow.Graphics.FillCircle(x, y, 45, blackBrush);
				OverlayWindow.Graphics.FillCircle(x, y, 40, brushColor);
				x += 95;
			}
		}


		private void DisplayGreenLight() {
			DrawStartLight(greenBrush);
			log.Info("Displaying Green Light");
		}

		private float GetSpeedPercentage() {
			var speedInKmh = GetSpeedInKmh(R3EData.myData.CarSpeed);
			var percentage = 100 / (float)allowedStartSpeed * speedInKmh;
			return percentage;
		}

		private float GetSpeedInKmh(float speed) {
			var speedInKmh = speed * 3.6f;
			return speedInKmh;
		}

		private float ComputeRemainingLapDistance() {
			var distanceDriven = PoleSetter.LapDistance;
			var totalLapDistance = R3EData.myData.LayoutLength;
			return totalLapDistance - distanceDriven;
		}

		public override void Dispose() {
			OverlayWindow.Dispose();
			base.Dispose();
		}

		private void ClearScreen() {
			OverlayWindow.Graphics.BeginScene();
			OverlayWindow.Graphics.ClearScene();
			OverlayWindow.Graphics.EndScene();
		}

	}
}