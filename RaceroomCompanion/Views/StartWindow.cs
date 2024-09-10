using R3EStart;
using RaceroomCompanion.Overlays.Helpers;
using RaceroomCompanion.Overlays.StartLight;
using System.Windows.Forms;

public class StartWindow : Form {
	private Button StartOverlayButton;
	private StartLightOverlayApplication MyStartLightOverlay;

	public StartWindow() {
		this.Width = 400;
		this.Height = 600;
		this.ShowInTaskbar = true;
		//this.Icon = Resources.R3EStart;
		this.Text = $"{Constants.RaceroomCompanion} - {this.ProductVersion}";
		this.InitializeComponent();
	}

	private void InitializeComponent() {
		this.StartOverlayButton = new System.Windows.Forms.Button();
		this.SuspendLayout();
		// 
		// button1
		// 
		this.StartOverlayButton.Location = new System.Drawing.Point(110, 89);
		this.StartOverlayButton.Name = "Startlight";
		this.StartOverlayButton.Size = new System.Drawing.Size(75, 23);
		this.StartOverlayButton.TabIndex = 0;
		this.StartOverlayButton.Text = "Startlight";
		this.StartOverlayButton.UseVisualStyleBackColor = true;
		this.StartOverlayButton.Click += new System.EventHandler(this.StartLights_Click);
		// 
		// StartWindow
		// 
		this.ClientSize = new System.Drawing.Size(284, 261);
		this.Controls.Add(this.StartOverlayButton);
		this.Name = "StartWindow";
		this.ResumeLayout(false);

	}

	private void StartLights_Click(object sender, System.EventArgs e) {
		var startLightOverlay = new StartLightOverlayApplication(Constants.R3EName64);
	}

}
