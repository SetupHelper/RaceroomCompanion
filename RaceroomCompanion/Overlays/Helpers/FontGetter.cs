using System;
using System.Drawing.Text;
using System.Linq;

namespace R3EStart.Overlays.Helpers {

	internal class FontCollector {

		private readonly InstalledFontCollection fontCollection;
		internal string NeedForFont => fontCollection != null && fontCollection.Families.Any(x => x.Name == "Need for Font") ? "Need for Font" : "Segoe UI";
		internal string Arial => "Arial";

		public FontCollector() {
			fontCollection = new InstalledFontCollection();
		}

	}

}
