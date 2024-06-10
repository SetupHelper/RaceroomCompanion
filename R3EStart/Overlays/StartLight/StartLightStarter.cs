using Process.NET;
using Process.NET.Memory;
using ReStart.R3E;


namespace R3EStart.Overlays.StartLight {
	public class StartLightStarter {

		public void Start(string processName) {
			var process = Utilities.WaitUntilSimIsRunning(processName);
			var _processSharp = new ProcessSharp(process, MemoryType.Remote);
			var startLightPlugin = new StartLightOverlay();
			startLightPlugin.Initialize(_processSharp.WindowFactory.MainWindow);
			startLightPlugin.Enable();
			while (Utilities.IsRrreRunning()) {
				startLightPlugin.Update();
			}
		}

	}
}