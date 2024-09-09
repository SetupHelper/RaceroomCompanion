using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Memory;
using Process.NET;
using Process.NET.Windows;
using System;
using RaceroomSharedMemory.R3E;

namespace RaceroomCompanion.Overlays.OverlayView {
	public abstract class RaceroomCompanionOverlayView<TApplication> : DirectXOverlayPlugin
		where TApplication : RaceroomCompanionOverlayApplication {

		internal TApplication MyApp { get; private set; }
		private readonly TickEngine MyTickEngine = new TickEngine();
		private int fps = 90;
		private int UpdateRate => 1000 / fps;
		internal int LargeFont { get; private set; }
		internal int SmallFont { get; private set; }
		internal int LargeNeedForFont { get; private set; }
		internal int SmallNeedForFont { get; private set; }
		internal int RedBrush { get; private set; }
		internal int YellowBrush { get; private set; }
		internal int YellowSolidBrush { get; private set; }
		internal int GreenBrush { get; private set; }
		internal int BlackBrush { get; private set; }
		internal int WhiteBrush { get; private set; }

		protected RaceroomCompanionOverlayView(TApplication app) : base() {
			MyApp = app;
			this.Initialize(MyApp.HookWindow);
			CreateBrushesAndFonts();
			this.Enable();
			while (Utilities.IsRrreRunning()) {
				this.Update();
			}
		}

		private void CreateBrushesAndFonts() {
			var transparency = 160;
			GreenBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 0, 100, 0));
			YellowBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 204, 204, 0));
			YellowSolidBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(0, 204, 204, 0));
			RedBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, 204, 0, 0));
			BlackBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, System.Drawing.Color.Black));
			WhiteBrush = OverlayWindow.Graphics.CreateBrush(System.Drawing.Color.FromArgb(transparency, System.Drawing.Color.White));
			LargeFont = OverlayWindow.Graphics.CreateFont(this.MyApp.FontGetter.Arial, 55);
			SmallFont = OverlayWindow.Graphics.CreateFont(this.MyApp.FontGetter.Arial, 40);
			LargeNeedForFont = OverlayWindow.Graphics.CreateFont(this.MyApp.FontGetter.NeedForFont, 55);
			SmallNeedForFont = OverlayWindow.Graphics.CreateFont(this.MyApp.FontGetter.NeedForFont, 40);
		}

		public override void Initialize(IWindow targetWindow) {
			base.Initialize(targetWindow);
			OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
			MyTickEngine.PreTick += OnPreTick;
			MyTickEngine.Tick += OnTick;
		}

		internal abstract void OnTick(object sender, EventArgs e);
		internal abstract void OnPreTick(object sender, EventArgs e);

		public override void Enable() {
			MyTickEngine.Interval = UpdateRate.Milliseconds();
			MyTickEngine.IsTicking = true;
			base.Enable();
		}

		public override void Disable() {
			MyTickEngine.IsTicking = false;
			base.Disable();
		}

		public override void Update() {
			MyTickEngine.Pulse();
		}

		public override void Dispose() {
			OverlayWindow.Dispose();
			base.Dispose();
		}

		internal void ClearScreen() {
			OverlayWindow.Graphics.BeginScene();
			OverlayWindow.Graphics.ClearScene();
			OverlayWindow.Graphics.EndScene();
		}


	}
}
