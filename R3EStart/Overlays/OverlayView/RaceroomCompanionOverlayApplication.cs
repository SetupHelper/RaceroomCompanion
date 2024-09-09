using Process.NET.Memory;
using Process.NET;
using RaceroomSharedMemory.R3E;
using Process.NET.Windows;
using R3EStart.Overlays.Helpers;

namespace RaceroomCompanion.Overlays.OverlayView {

	public abstract class RaceroomCompanionOverlayApplication {
		
		public IWindow HookWindow { get; }
		internal readonly FontCollector FontGetter;

		public RaceroomCompanionOverlayApplication(string processName) {
			var process = Utilities.WaitUntilSimIsRunning(processName);
			var _processSharp = new ProcessSharp(process, MemoryType.Remote);
			HookWindow = _processSharp.WindowFactory.MainWindow;
			FontGetter = new FontCollector();
		}

	}

}
