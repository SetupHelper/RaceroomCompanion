using Newtonsoft.Json;
using RaceroomSharedMemory.R3E;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PenaltyProcessor.Objects.TrackVectorCollection;

namespace PenaltyProcessor.Objects {
	public class TrackZone {

		[JsonProperty]
		public List<Vector2<double>> LaneLeftCoordinates { get; internal set; }
		[JsonProperty]
		public List<Vector2<double>> LineRightCoordinates { get; internal set; }
		public List<Vector2<double>> ZoneCoordinates {
			get {
				var coords = new List<Vector2<double>>();
				coords.AddRange(LaneLeftCoordinates);
				coords.AddRange(LineRightCoordinates.AsEnumerable().Reverse().ToList());
				coords.Add(LaneLeftCoordinates.FirstOrDefault());
				return coords;
			}
		}

		[JsonConstructor]
		public TrackZone() {
			LaneLeftCoordinates = new List<Vector2<double>>();
			LineRightCoordinates = new List<Vector2<double>>();
		}

		public void RecordLeftCoordinates(R3EData data) {
			var coord = new Vector2<double>() {
				X = data.MyData.Player.Position.X,
				Y = data.MyData.Player.Position.Y
			};
			this.LaneLeftCoordinates.Add(coord);
		}

		public void RecordRightCoordinates(R3EData data) {
			var coord = new Vector2<double>() {
				X = data.MyData.Player.Position.X,
				Y = data.MyData.Player.Position.Y
			};
			this.LineRightCoordinates.Add(coord);
		}

	}
}
