using R3EStart.Overlays.StartLight;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R3EStart {
	/// <summary>
	/// </summary>
	public static class Program {
		/// <summary>
		///     Defines the entry point of the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			Task.Run(() => {
				var startLightOverlay = new StartLightStarter();
				startLightOverlay.Start("RRRE64");
			});
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new StartWindow());
		}
	}
}