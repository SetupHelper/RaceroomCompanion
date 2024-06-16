using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;
using ReStart.Helpers;
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
		private int greenLightDistance;
		private int allowedStartSpeed = 0;
		private int yCorrection => this.OverlayWindow.Height / 4;
		private int WidthDependingOnScreenHeight => this.OverlayWindow.Height / 8;
		private DriverData PoleSetter => R3EData.myData.DriverData.FirstOrDefault();
		//private static readonly ILog log = LogManager.GetLogger(typeof(Program) );
		private static readonly ILog log = LogManager.GetLogger("RollingFileAppender");

		public override void Initialize(IWindow targetWindow) {
			log.Info("Starting R3EStart overlay");
			base.Initialize(targetWindow);
			R3EData = Utilities.MapData();
			int seed = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
			Random random = new Random(seed);
			greenLightDistance = new Random(seed).Next(5, 120);
			log.Info($"greenLightDistance: {greenLightDistance}");
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
			int centerX = this.TargetWindow.Width / 2;
			int centerY = this.TargetWindow.Height / 2 - yCorrection;
			foreach (var item in R3EData.myData.DriverData.Where(x=>x.DriverInfo.SlotId >= 0)) {
				OverlayWindow.Graphics.DrawText($"{item.DriverInfo.SlotId} / {item.Place}", largeFont, blackBrush, centerX, centerY);
				centerY += 40;
			}
			OverlayWindow.Graphics.DrawText($"{R3EData.myData.DriverData[0].DriverInfo.SlotId} / {R3EData.myData.DriverData[0].Place}", largeFont, blackBrush, centerX, centerY);
			OverlayWindow.Graphics.DrawText($"{R3EData.myData.DriverData[1].DriverInfo.SlotId} / {R3EData.myData.DriverData[1].Place}", largeFont, blackBrush, centerX, centerY + 40);
			OverlayWindow.Graphics.DrawText($"{R3EData.myData.DriverData[2].DriverInfo.SlotId} / {R3EData.myData.DriverData[2].Place}", largeFont, blackBrush, centerX, centerY + 80);
			OverlayWindow.Graphics.DrawText($"{R3EData.myData.DriverData[2].DriverInfo.SlotId} / {R3EData.myData.DriverData[2].Place}", largeFont, blackBrush, centerX, centerY + 80);
			if (DisplayOverlays()) {
				if (remainingLapDistance > 1000) {
					DisplaySafetyCarSign();
				}
				if (remainingLapDistance <= 1000 && remainingLapDistance > greenLightDistance) {
					DisplaySpeedLimiter();
				}
				if (remainingLapDistance <= greenLightDistance) {
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

		private void DisplayGreenLight() {
			OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 45, blackBrush);
			OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 40, greenBrush);
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