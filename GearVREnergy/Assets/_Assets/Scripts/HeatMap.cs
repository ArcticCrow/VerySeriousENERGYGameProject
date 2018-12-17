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

	bool needsRedraw = false;

	private void Update()
	{
		for (int i = 0; i < roomMappings.Count; i++)
		{
			if (roomMappings[i].room.energyLevelUpdate)
			{
				needsRedraw = true;
			}
			roomMappings[i].room.energyLevelUpdate = false;
		}
		if (needsRedraw)
		{
			RedrawMap();
		}
	}

	internal virtual void RedrawMap() { needsRedraw = false; }
}
