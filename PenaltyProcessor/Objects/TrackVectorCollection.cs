using Newtonsoft.Json;
using RaceroomSharedMemory.R3E;
using System.Collections.Generic;
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
		public double penaltyLapStart { get; set; }
		[JsonProperty]
		public double penaltyLapEnd { get; set; }
		[JsonProperty]
		public TrackZone PenaltyLane { get; internal set; }
		[JsonProperty]
		public TrackZone LeftStartLane{ get; internal set; }
		[JsonProperty]
		public TrackZone RightStartLane { get; internal set; }

		[JsonConstructor]
		private TrackVectorCollection() {

		}

		public TrackVectorCollection(R3EData data) {
			this.PenaltyLane = new TrackZone();
			this.LeftStartLane = new TrackZone();
			this.RightStartLane = new TrackZone();
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

		//public void RecordCoordinates(R3EData data) {
		//	if (this.penaltyLapStart <= 0) {
		//		this.penaltyLapStart = data.MyData.LapDistance;
		//		this.PenaltyLaneCoordinates = new List<Vector2<double>>();
		//	}
		//	var test = new Vector2<double>();
		//	test.X = data.MyData.Player.Position.X;
		//	test.Y = data.MyData.Player.Position.Y;
		//	this.PenaltyLaneCoordinates.Add(test);
		//}

		public void WriteDataToConfigFile() {
			var filePath = configFilePath;
			if (!File.Exists(filePath)) {
				using (var fileStream = File.Create(filePath)) { }
			}
			using (var file = File.OpenWrite(filePath)) {
				using (var writer = new StreamWriter(file)) {
					writer.WriteLine(JsonConvert.SerializeObject(this));
				}
			}
		}

		private void LoadConfig() {
			if (File.Exists(configFilePath)) {
				var configContent = File.ReadAllText(configFilePath);
				var values = JsonConvert.DeserializeObject<TrackVectorCollection>(configContent);
				this.penaltyLapStart = values.penaltyLapStart;
				this.penaltyLapEnd = values.penaltyLapEnd;
				this.PenaltyLane = values.PenaltyLane;
				this.LeftStartLane = values.LeftStartLane;
				this.RightStartLane = values.RightStartLane;	
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
