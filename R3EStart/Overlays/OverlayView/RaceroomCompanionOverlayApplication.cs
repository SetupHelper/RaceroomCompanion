using Process.NET.Memory;
using Process.NET;
using RaceroomSharedMemory.R3E;
using Process.NET.Windows;

namespace RaceroomCompanion.Overlays.OverlayView {

	public abstract class RaceroomCompanionOverlayApplication {
		
		public IWindow HookWindow { get; }
		
		public RaceroomCompanionOverlayApplication(string processName) {
			var process = Utilities.WaitUntilSimIsRunning(processName);
			var _processSharp = new ProcessSharp(process, MemoryType.Remote);
			HookWindow = _processSharp.WindowFactory.MainWindow;
		}

	}

}
