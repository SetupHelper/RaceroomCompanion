using RaceroomCompanion.Overlays.Helpers;
using RaceroomCompanion.Overlays.StartLight;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaceroomCompanion {
	/// <summary>
	/// </summary>
	public static class Program {
		/// <summary>
		///     Defines the entry point of the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			Task.Run(() => {
				var test = new StartLightOverlayApplication(Constants.R3EName64);
				test.Start();
			}); 
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new StartWindow());
		}
	}
}