using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;
using ReStart.Helpers;
using ReStart.R3E;

namespace R3EStart.Overlays.StartLight {

	public class StartLightOverlay : DirectXOverlayPlugin {
		private readonly TickEngine _tickEngine = new TickEngine();
		private int _displayFps;
		private int _font;
		private int _hugeFont;
		private int _i;
		private int _interiorBrush;
		private int _redBrush;
		private int _greenBrush;
		private int _blackBrush;
		private int _redOpacityBrush;
		private float _rotation;
		private Stopwatch _watch;
		private int fps = 60;
		public int UpdateRate => 1000 / fps;
		private R3EData _R3EData;
		private int _greenLightDistance;

		public override void Initialize(IWindow targetWindow) {
			// Set target window by calling the base method
			base.Initialize(targetWindow);
			_R3EData = Utilities.MapData();
			// For demo, show how to use settings
			var type = GetType();
			_greenLightDistance = new Random().Next(0, 120);


			OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
			_watch = Stopwatch.StartNew();

			_redBrush = OverlayWindow.Graphics.CreateBrush(0x7FFF0000);
			_greenBrush = OverlayWindow.Graphics.CreateBrush(Color.FromKnownColor(KnownColor.Green));
			_blackBrush = OverlayWindow.Graphics.CreateBrush(Color.FromKnownColor(KnownColor.Black));
			_redOpacityBrush = OverlayWindow.Graphics.CreateBrush(Color.FromArgb(80, 255, 0, 0));
			_interiorBrush = OverlayWindow.Graphics.CreateBrush(0x7FFFFF00);

			_font = OverlayWindow.Graphics.CreateFont("Arial", 20);
			_hugeFont = OverlayWindow.Graphics.CreateFont("Arial", 50, true);

			_rotation = 0.0f;
			_displayFps = 0;
			_i = 0;
			// Set up update interval and register events for the tick engine.

			_tickEngine.PreTick += OnPreTick;
			_tickEngine.Tick += OnTick;
		}

		private void OnTick(object sender, EventArgs e) {
			if (!OverlayWindow.IsVisible) {
				return;
			}
			_R3EData.Read();
			OverlayWindow.Update();
			InternalRender();
		}

		private void OnPreTick(object sender, EventArgs e) {
			var targetWindowIsActivated = TargetWindow.IsActivated;
			if (!targetWindowIsActivated && OverlayWindow.IsVisible) {
				_watch.Stop();
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

		protected void InternalRender() {
			OverlayWindow.Graphics.BeginScene();
			OverlayWindow.Graphics.ClearScene();
			if (_R3EData.myData.SessionType == (int)Session.Race && _R3EData.myData.GameInMenus == 0 && _R3EData.myData.CompletedLaps == 0) {
				var distance = ComputeRemainingDistance();
				if (distance > 500) {
					OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 45, _blackBrush);
					//OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 40, _redBrush);
				}
				if (distance <= 500 && distance > _greenLightDistance) {
					OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 45, _blackBrush);
					OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 40, _redBrush);
				}
				//if (distance <= _greenLightDistance) {
				//	OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 45, _blackBrush);
				//	OverlayWindow.Graphics.FillCircle(this.TargetWindow.Width / 2, this.TargetWindow.Height / 2, 40, _greenBrush);
				//}
			}
			OverlayWindow.Graphics.EndScene();
		}

		private float ComputeRemainingDistance() {
			var distanceDriven = _R3EData.myData.LapDistance;
			var totalLapDistance = _R3EData.myData.LayoutLength;
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

		internal void SetSimData(R3EData r3EData) {

		}

	}
}