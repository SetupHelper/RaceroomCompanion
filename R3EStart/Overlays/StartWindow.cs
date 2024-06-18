using R3EStart;
using R3EStart.Helpers;
using System.Windows.Forms;

public class StartWindow : Form {
	public StartWindow() {
		this.Width = 280;
		this.Height = 0;
		this.ShowInTaskbar = true;
		this.Icon = Resources.R3EStart;
		this.Text = $"{Constants.ReStartName} - {this.ProductVersion}";
	}
}
