﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramHeatMap : HeatMap {

	public List<string> colorChannels = new List<string>() {
		"_RimColor",
		"_MainColor"
	};

	public override void RedrawMap()
	{
		base.RedrawMap();
		
		for (int i = 0; i < roomMappings.Count; i++)
		{
			RoomMapping map = roomMappings[i];

<<<<<<< HEAD
				for (int j = 0; j < colorChannels.Count; j++)
				{
					mat.SetColor(
						colorChannels[j], 
						map.room.energyLevel.warningColor
						);
				}
=======
			Material mat = map.heatmapRoom.GetComponentInChildren<MeshRenderer>().material;

			for (int j = 0; j < colorChannels.Count; j++)
			{
				mat.SetColor(colorChannels[j], map.room.influenceLevel.warningColor);
>>>>>>> parent of 3ca0e00... Merge branch 'Development' of https://github.com/ArcticCrow/VerySeriousENERGYGameProject into Development
			}
			
		}
	}
}
