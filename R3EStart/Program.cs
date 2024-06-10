using R3EStart.Overlays.StartLight;
using System;

namespace Overlay.NET.Demo {
	/// <summary>
	/// </summary>
	public static class Program {
		/// <summary>
		///     Defines the entry point of the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			var startLightOverlay = new StartLightStarter();
			startLightOverlay.Start("RRRE64");
		}
	}
}