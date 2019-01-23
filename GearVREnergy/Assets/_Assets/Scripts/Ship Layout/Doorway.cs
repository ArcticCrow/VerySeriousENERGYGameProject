using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour {

	public bool openable = true;

	public RoomInformation roomA;
	public RoomInformation roomB;

	public float highlightAlpha = 0.3f;
	public GameObject highlightA;
	public GameObject highlightB;

	public GameObject door;

	private Material hlMatA, hlMatB;
	bool open = false;
	float openTime;
	float lastOpenTime;

	// Use this for initialization
	void Start () {
		if (door == null)
		{
			door = gameObject.GetComponentInChildren<Animator>().gameObject;
		}

		if (highlightA != null)
		{
			hlMatA = highlightA.GetComponent<MeshRenderer>().material;
		}
		if (highlightB != null)
		{
			hlMatB = highlightB.GetComponent<MeshRenderer>().material;
		}
	}

	void ToggleHighlightColor(bool showHighlights)
	{
		if (highlightA != null && highlightB != null)
		{
			if (showHighlights && highlightAlpha > 0)
			{
				hlMatA.color = new Color(roomA.colorCode.r, roomA.colorCode.g, roomA.colorCode.b, highlightAlpha);
				hlMatB.color = new Color(roomB.colorCode.r, roomB.colorCode.g, roomB.colorCode.b, highlightAlpha);
			}
			else
			{
				hlMatA.color = new Color(roomA.colorCode.r, roomA.colorCode.g, roomA.colorCode.b, 0.01f);
				hlMatB.color = new Color(roomB.colorCode.r, roomB.colorCode.g, roomB.colorCode.b, 0.01f);
			}
		}
		
	}

	public void ChangeRoom()
	{
		GameManager.instance.TeleportToOtherRoom(roomA, roomB);
	}

	public void OpenDoor(float keepOpenTime)
	{
		lastOpenTime = Time.time;
		openTime = keepOpenTime;

		ToggleHighlightColor(true);

		if (!open)
		{
			open = true;
			if (door != null)
			{
				SFXController.PlaySound(SFXController.instance.soundEffects.door);
				door.GetComponent<Animator>().SetBool("LookAt", true);
			}
		}
	}

	public void CloseDoor(bool force = false)
	{
		if (lastOpenTime != Time.time)
		{
			ToggleHighlightColor(false);
		}
		if (force || (open && (Time.time - lastOpenTime) > openTime))
		{
			open = false;
			if (door != null)
			{
				door.GetComponent<Animator>().SetBool("LookAt", false);
			}
			
		}
	}

	private void Update()
	{
		CloseDoor();
	}
}
