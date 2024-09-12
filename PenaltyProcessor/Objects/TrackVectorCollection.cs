using Newtonsoft.Json;
using RaceroomSharedMemory.R3E;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace PenaltyProcessor.Objects {

	public class TrackVectorCollection {

		private string configFilePath => Path.Combine(Directory.GetCurrentDirectory(), "TrackConfig", $"{this.TrackName}_{this.TrackId}_{this.LayoutId}.json");

		[JsonProperty] 
		public int TrackId { get; private set; }
		[JsonProperty] 
		public int LayoutId { get; private set; }
		[JsonProperty] 
		public string TrackName { get; private set; }
		[JsonProperty]
		public double penaltyLapStart { get; private set; }
		[JsonProperty]
		public double penaltyLapEnd { get; private set; }
		[JsonProperty]
		public Vector2<double> LocationVector { get; private set; }
		[JsonProperty]
		public Vector2<double> DirectionalVectorInRaceDirection { get; private set; }
		[JsonProperty]
		public Vector2<double> DirectionalVectorLeftToRight { get; private set; }

		[JsonConstructor]
		private TrackVectorCollection() {
		
		}

		public TrackVectorCollection(R3EData data) {
			this.TrackId = data.MyData.TrackId;
			this.LayoutId = data.MyData.LayoutId;
			this.TrackName = ConvertTrackNameToString(data.MyData.TrackName);
			if (!string.IsNullOrEmpty(TrackName)) {
				this.LoadConfig();
			}
		}

		internal static string ConvertTrackNameToString(byte[] trackName) {
			if (trackName == null || trackName.All(x => x <= 0)) {
				return string.Empty;
			}
			var builder = new StringBuilder();
			foreach (var letter in Encoding.UTF8.GetString(trackName)) {
				if (letter <= 0) {
					break;
				}
				builder.Append(letter);
			}
			return builder.ToString();
		}

		private void WriteDataToConfigFile() {


			this.LocationVector = new Vector2<double> {
				X = -1348.031,
				Y = 102.0803
			};
			this.DirectionalVectorInRaceDirection = new Vector2<double> {
				X = -1328.261,
				Y = 104.4304
			};
			this.DirectionalVectorLeftToRight = new Vector2<double> {
				X = 1343.501,
				Y = 120.0416
			};
			this.penaltyLapStart = 725;
			this.penaltyLapEnd = 765;
			if (!File.Exists(configFilePath)) {
				using (var fileStream = File.Create(configFilePath)) { }
			}
			using (var file = File.OpenWrite(configFilePath)) {
				using (var writer = new StreamWriter(file)) {
					writer.WriteLine(JsonConvert.SerializeObject(this));
				}
			}
		}

		private void LoadConfig() {
			if (File.Exists(configFilePath)) {
				var configContent = File.ReadAllText(configFilePath);
				var values = JsonConvert.DeserializeObject<TrackVectorCollection>(configContent);
				this.LocationVector = values.LocationVector;
				this.DirectionalVectorInRaceDirection = values.DirectionalVectorInRaceDirection;
				this.DirectionalVectorLeftToRight = values.DirectionalVectorLeftToRight;
				this.penaltyLapStart = values.penaltyLapStart;
				this.penaltyLapEnd = values.penaltyLapEnd;
			}
		}

		internal void SaveConfig() {
			File.WriteAllText(configFilePath, JsonConvert.SerializeObject(this));
		}

		public struct Vector2<T> {
			public T X;
			public T Y;
		}

	}

}
