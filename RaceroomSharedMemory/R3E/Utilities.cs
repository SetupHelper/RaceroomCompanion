using RaceroomSharedMemory.Objects;
using System.Linq;
using System.Threading;

namespace RaceroomSharedMemory.R3E {
	public class Utilities {

		public static bool IsRrreRunningIn64() {
			return System.Diagnostics.Process.GetProcessesByName(Constants.R3EName64).Length > 0;
		}
		public static bool IsRrreRunningIn32() {
			return System.Diagnostics.Process.GetProcessesByName(Constants.R3EName32).Length > 0;
		}

		public static bool IsRrreRunning() {
			return IsRrreRunningIn64() || IsRrreRunningIn32();
		}
		public static bool IsReStartRunning() {
			return System.Diagnostics.Process.GetProcessesByName(Constants.ReStartName).Length > 1;
		}

		public static R3EData MapData() {
			var r3eData = new R3EData();
			r3eData.Map();
			while (!r3eData.Mapped) {
				Thread.Sleep(5000);
			}
			return r3eData;
		}

		public static System.Diagnostics.Process WaitUntilSimIsRunning(string processName) {
			var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
			while (process == null) {
				Thread.Sleep(2000);
				process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
			}
			return process;
		}



	}
}