//using Process.NET;
//using Process.NET.Memory;
//using RaceroomCompanion.Overlays.OverlayView;
//using RaceroomSharedMemory.R3E;

//namespace RaceroomCompanion.Overlays {
//	public class OverlayStarter<TApplication, TOverlay>
//		where TApplication : RaceroomCompanionOverlayApplication
//		where TOverlay : RaceroomCompanionOverlayView<TApplication>, new() {

//		public void Start(string processName, TApplication app) {
//			var process = Utilities.WaitUntilSimIsRunning(processName);
//			var _processSharp = new ProcessSharp(process, MemoryType.Remote);
//			var overlay = new TOverlay();
//			overlay.SetApplication(app);
//			overlay.Initialize(_processSharp.WindowFactory.MainWindow);
//			overlay.Enable();
//			while (Utilities.IsRrreRunning()) {
//				overlay.Update();
//			}
//		}

//	}
//}