using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap : MonoBehaviour {

	[Serializable]
	public struct RoomMapping
	{
		public GameObject heatmapRoom;
		public RoomInformation room;

		public RoomMapping(GameObject heatmapRoom, RoomInformation room)
		{
			this.heatmapRoom = heatmapRoom;
			this.room = room;
		}
	}

	public List<RoomMapping> roomMappings = new List<RoomMapping>();

    public virtual void RedrawMap() { }
}
