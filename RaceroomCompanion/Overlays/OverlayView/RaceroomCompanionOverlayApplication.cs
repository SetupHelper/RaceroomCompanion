using Process.NET.Memory;
using Process.NET;
using RaceroomSharedMemory.R3E;
using Process.NET.Windows;
using R3EStart.Overlays.Helpers;
using System.Threading;

namespace RaceroomCompanion.Overlays.OverlayView {

	public abstract class RaceroomCompanionOverlayApplication {

		public IWindow HookWindow { get; }
		internal readonly FontCollector FontGetter;
		internal bool IsRunning { get; set; }
		internal readonly CancellationTokenSource MyCancellationTokenSource;
		internal CancellationToken CancelToken => MyCancellationTokenSource.Token;

		public RaceroomCompanionOverlayApplication(string processName) {
			var process = Utilities.WaitUntilSimIsRunning(processName);
			var _processSharp = new ProcessSharp(process, MemoryType.Remote);
			HookWindow = _processSharp.WindowFactory.MainWindow;
			IsRunning = true;
			MyCancellationTokenSource = new CancellationTokenSource();
			FontGetter = new FontCollector();
		}

	}

}
