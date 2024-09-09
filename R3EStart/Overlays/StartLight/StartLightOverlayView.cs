using System;
using System.Threading;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;
using RaceroomCompanion.Overlays.OverlayView;

namespace RaceroomCompanion.Overlays.StartLight {

	public class StartLightOverlayView : RaceroomCompanionOverlayView<StartLightOverlayApplication> {

		private readonly TickEngine _tickEngine = new TickEngine();
		private int largeFont;
		private int smallFont;
		private int redBrush;
		private int yellowBrush;
		private int greenBrush;
		private int blackBrush;
		private int whiteBrush;
		private int MyYCorrection => this.OverlayWindow.Height / 4;
		private int MyWidthDependingOnScreenHeight => this.OverlayWindow.Height / 8;

		public StartLightOverlayView(StartLightOverlayApplication app) : base(app) {
		}

		public override void Initialize(IWindow targetWindow) {
			base.Initialize(targetWindow);
			OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
			CreateBrushesAndFonts();
			_tickEngine.PreTick += OnPreTick;
			_tickEngine.Tick += OnTick;
		}

		private void CreateBrushesAndFonts() {
			var transparency = 160;
			greenBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 0, 100, 0));
			yellowBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 204, 204, 0));
			redBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 204, 0, 0));
			blackBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, System.Drawing.Color.Black));
			whiteBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, System.Drawing.Color.White));
			largeFont = OverlayWindow.Graphics.CreateFont("Arial", 55);
			smallFont = OverlayWindow.Graphics.CreateFont("Arial", 40);
		}

		internal override void OnTick(object sender, EventArgs e) {
			try {
				if (!OverlayWindow.IsVisible) {
					return;
				}
				this.MyApp.UpdateData();
				OverlayWindow.Graphics.BeginScene();
				OverlayWindow.Graphics.ClearScene();
				OverlayWindow.Update();
				RenderOverlays();
				OverlayWindow.Graphics.EndScene();
				if (this.MyApp.PauseForGreenLight) {
					Thread.Sleep(4000);
					this.MyApp.StopPauseForGreenLight();
				}
			} catch (Exception) {
			}
		}

		internal override void OnPreTick(object sender, EventArgs e) {
			var targetWindowIsActivated = TargetWindow.IsActivated;
			if (!targetWindowIsActivated && OverlayWindow.IsVisible) {
				ClearScreen();
				OverlayWindow.Hide();
			} else if (targetWindowIsActivated && !OverlayWindow.IsVisible) {
				OverlayWindow.Show();
			}
		}

		internal void RenderOverlays() {
			if (this.MyApp.DisplayOverlays()) {
				if (this.MyApp.RenderSafetyCarSign()) {
					this.DisplaySafetyCarSign();
				} else if (this.MyApp.RenderSpeedLimiter()) {
					this.DisplaySpeedLimiter();
				} else if (this.MyApp.RenderRedLight()) {
					this.DisplaySpeedLimiter();
					this.DisplayRedLight();
				} else if (this.MyApp.RenderGreenLight()) {
					_tickEngine.Stop();
					DisplayGreenLight();
				}
			}
		}

		internal void DisplaySafetyCarSign() {
			int centerX = this.TargetWindow.Width / 2;
			int centerY = this.TargetWindow.Height / 2 - MyYCorrection + 100;
			OverlayWindow.Graphics.BorderedRectangle(centerX, centerY, MyWidthDependingOnScreenHeight, MyWidthDependingOnScreenHeight, 2, 10, blackBrush, yellowBrush);
		}

		internal void DisplaySpeedLimiter() {
			DrawSpeedLimitSign();
			DrawSpeedBar();
		}

		internal void DrawSpeedLimitSign() {
			int centerX = this.TargetWindow.Width / 2;
			int centerY = this.TargetWindow.Height / 2 - MyYCorrection;
			int outerRadius = 44;
			int innerRadius = 40;
			OverlayWindow.Graphics.FillCircle(centerX, centerY, outerRadius, whiteBrush);
			OverlayWindow.Graphics.DrawCircle(centerX, centerY, outerRadius, 10, redBrush);
			OverlayWindow.Graphics.FillCircle(centerX, centerY, innerRadius, whiteBrush);
			OverlayWindow.Graphics.DrawText(this.MyApp.AllowedStartSpeed.ToString(), largeFont, blackBrush, centerX - 32, centerY - 32);
		}

		internal void DrawSpeedBar() {
			int barWidth = 600;
			int centerX = this.TargetWindow.Width / 2 - barWidth / 2;
			int centerY = this.TargetWindow.Height / 2 - MyYCorrection + 70;
			var barColor = greenBrush;
			var speedInKmh = this.MyApp.GetCarSpeed();
			if (speedInKmh > this.MyApp.AllowedStartSpeed + 4) {
				barColor = yellowBrush;
			}
			if (speedInKmh > this.MyApp.AllowedStartSpeed + 6) {
				barColor = redBrush;
			}
			OverlayWindow.Graphics.DrawBarV(centerX, centerY, 600, 50, this.MyApp.GetSpeedPercentage(), 1, blackBrush, barColor);
			OverlayWindow.Graphics.DrawText($"{(int)speedInKmh} km/h", smallFont, blackBrush, centerX, centerY);
		}

		internal void DisplayRedLight() {
			DrawStartLight(redBrush);
		}

		internal void DrawStartLight(int brushColor) {
			var x = this.TargetWindow.Width / 2 - 200;
			var y = this.TargetWindow.Height / 2 - MyYCorrection + 180;
			for (int i = 0; i < 5; i++) {
				OverlayWindow.Graphics.FillCircle(x, y, 45, blackBrush);
				OverlayWindow.Graphics.FillCircle(x, y, 40, brushColor);
				x += 95;
			}
		}

		internal void DisplayGreenLight() {
			DrawStartLight(greenBrush);
		}

	}
}