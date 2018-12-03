using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInformation : MonoBehaviour {
	private static int nextID = 0;

	[HideInInspector]
	public int id = nextID++;

	public string roomName = "Unnamed";
	public Color colorCode = Color.white;
}
