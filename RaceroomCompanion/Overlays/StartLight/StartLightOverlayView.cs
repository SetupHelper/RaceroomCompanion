using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;
using RaceroomCompanion.Overlays.OverlayView;
using static PenaltyProcessor.Objects.TrackVectorCollection;

namespace RaceroomCompanion.Overlays.StartLight {

	public class StartLightOverlayView : RaceroomCompanionOverlayView<StartLightOverlayApplication> {

		private readonly TickEngine _tickEngine = new TickEngine();

		private int MyYCorrection => this.OverlayWindow.Height / 4;
		private int MyWidthDependingOnScreenHeight => this.OverlayWindow.Height / 8;

		public StartLightOverlayView(StartLightOverlayApplication app) : base(app) {
		}

		public override void Initialize(IWindow targetWindow) {
			base.Initialize(targetWindow);
			OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
			_tickEngine.PreTick += OnPreTick;
			_tickEngine.Tick += OnTick;
		}

		internal override void OnTick(object sender, EventArgs e) {
			try {
				if (!OverlayWindow.IsVisible) {
					return;
				}
				if (this.MyApp.CancelToken.IsCancellationRequested) {
					this.Dispose();
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
			var targetWindowIsActivated = this.TargetWindow.IsActivated;
			if (!targetWindowIsActivated && this.OverlayWindow.IsVisible) {
				ClearScreen();
				OverlayWindow.Hide();
			} else if (targetWindowIsActivated && !this.OverlayWindow.IsVisible) {
				OverlayWindow.Show();
			}
		}

		internal void RenderOverlays() {
			if (Debugger.IsAttached) {
				DisplayDebugInformation();
			}
			if (this.MyApp.DisplayOverlays()) {
				if (this.MyApp.RenderSafetyCarSign()) {
					this.DisplaySafetyCarSignNew();
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

		internal void DisplayDebugInformation() {
			int centerX = this.TargetWindow.Width / 2;
			int centerY = this.TargetWindow.Height / 2;
			var brush = MyApp.lapChecker.CurrentVectorIsInPenaltyLap(this.MyApp.R3EData) ? this.YellowSolidBrush: this.RedBrush ;
			OverlayWindow.Graphics.DrawText($"track: {MyApp.R3EData.MyData.DriverData.FirstOrDefault().LapDistance}", this.LargeNeedForFont, brush, centerX - 500, centerY - 100);
			OverlayWindow.Graphics.DrawText($"pos x: {MyApp.R3EData.MyData.DriverData.FirstOrDefault().Position.X}", this.LargeNeedForFont, brush, centerX - 500, centerY);
			OverlayWindow.Graphics.DrawText($"pos y: {MyApp.R3EData.MyData.DriverData.FirstOrDefault().Position.Y}", this.LargeNeedForFont, brush, centerX - 500, centerY + 100);
			OverlayWindow.Graphics.DrawText($"pos z: {MyApp.R3EData.MyData.DriverData.FirstOrDefault().Position.Z}", this.LargeNeedForFont, brush, centerX - 500, centerY + 200);
		}


		internal void DisplaySafetyCarSignNew() {
			int centerX = this.TargetWindow.Width / 2 - TargetWindow.Width / 6;
			int centerY = this.TargetWindow.Height / 4;
			OverlayWindow.Graphics.DrawText("SAFETY CAR", this.LargeNeedForFont, this.YellowSolidBrush, centerX - 500, centerY);
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
			OverlayWindow.Graphics.FillCircle(centerX, centerY, outerRadius, this.WhiteBrush);
			OverlayWindow.Graphics.DrawCircle(centerX, centerY, outerRadius, 10, this.RedBrush);
			OverlayWindow.Graphics.FillCircle(centerX, centerY, innerRadius, this.WhiteBrush);
			OverlayWindow.Graphics.DrawText(this.MyApp.AllowedStartSpeed.ToString(), this.LargeFont, this.BlackBrush, centerX - 32, centerY - 32);
		}

		internal void DrawSpeedBar() {
			int barWidth = 600;
			int centerX = this.TargetWindow.Width / 2 - barWidth / 2;
			int centerY = this.TargetWindow.Height / 2 - MyYCorrection + 70;
			var barColor = this.GreenBrush;
			var speedInKmh = this.MyApp.GetCarSpeed();
			if (speedInKmh > this.MyApp.AllowedStartSpeed + 4) {
				barColor = this.YellowBrush;
			}
			if (speedInKmh > this.MyApp.AllowedStartSpeed + 6) {
				barColor = this.RedBrush;
			}
			OverlayWindow.Graphics.DrawBarV(centerX, centerY, 600, 50, this.MyApp.GetSpeedPercentage(), 1, this.BlackBrush, barColor);
			OverlayWindow.Graphics.DrawText($"{(int)speedInKmh} km/h", this.SmallFont, this.BlackBrush, centerX, centerY);
		}

		internal void DisplayRedLight() {
			DrawStartLight(this.RedBrush);
		}

		internal void DrawStartLight(int brushColor) {
			var x = this.TargetWindow.Width / 2 - 200;
			var y = this.TargetWindow.Height / 2 - MyYCorrection + 180;
			for (int i = 0; i < 5; i++) {
				OverlayWindow.Graphics.FillCircle(x, y, 45, this.BlackBrush);
				OverlayWindow.Graphics.FillCircle(x, y, 40, brushColor);
				x += 95;
			}
		}

		internal void DisplayGreenLight() {
			DrawStartLight(this.GreenBrush);
		}

	}
}