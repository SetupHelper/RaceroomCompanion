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

		internal TApplication MyApp {get; private set;}
		private readonly TickEngine _tickEngine = new TickEngine();
		private int fps = 90;
		private int UpdateRate => 1000 / fps;


		protected RaceroomCompanionOverlayView(TApplication app) : base() {
			MyApp = app;
			this.Initialize(MyApp.HookWindow);
			this.Enable();
			while (Utilities.IsRrreRunning()) {
				this.Update();
			}
		}

		public override void Initialize(IWindow targetWindow) {
			base.Initialize(targetWindow);
			OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
			_tickEngine.PreTick += OnPreTick;
			_tickEngine.Tick += OnTick;
		}

		internal abstract void OnTick(object sender, EventArgs e);
		internal abstract void OnPreTick(object sender, EventArgs e);

		public override void Enable() {
			_tickEngine.Interval = UpdateRate.Milliseconds();
			_tickEngine.IsTicking = true;
			base.Enable();
		}

		public override void Disable() {
			_tickEngine.IsTicking = false;
			base.Disable();
		}

		public override void Update() {
			_tickEngine.Pulse();
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
