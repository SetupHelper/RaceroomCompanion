using RaceroomCompanion.Overlays.Helpers;
using RaceroomCompanion.Overlays.StartLight;
using System.Threading.Tasks;
using System.Windows.Forms;

public class StartWindow : Form {

	private StartLightOverlayApplication MyStartLightOverlay;

	public StartWindow() {
		this.Width = 400;
		this.Height = 600;
		this.ShowInTaskbar = true;
		//this.Icon = Resources.R3EStart;
		this.Text = $"{Constants.RaceroomCompanion} - {this.ProductVersion}";
		this.InitializeComponent();
		MyStartLightOverlay = new StartLightOverlayApplication(Constants.R3EName64);
	}

	private void InitializeComponent() {
		this.SuspendLayout();
		this.ClientSize = new System.Drawing.Size(284, 261);
		this.ResumeLayout(false);
	}

	private void StartOverlay_Click(object sender, System.EventArgs e) {
		if (MyStartLightOverlay.IsRunning) {
			MyStartLightOverlay.MyCancellationTokenSource.Cancel();
		}
		Task.Run(() => {
			MyStartLightOverlay.Start();
		});
	}

}
