using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

	public bool disableDoors = false;
	public float keepOpenTime = 0.5f;

	[SerializeField]
	List<GameObject> openableDoorways;

	Doorway currentDoorway;

	public void EnableDoorway(GameObject doorway)
	{
		if (openableDoorways == null)
		{
			openableDoorways = new List<GameObject>();
		}
		openableDoorways.Add(doorway);
	}

	public void DisableDoorway(GameObject doorway)
	{
		if (openableDoorways == null) return;
		openableDoorways.RemoveAll(x => x == doorway);
	}

	public void EnableAllDoorways(bool resetList = false)
	{
		disableDoors = false;
		if (resetList) openableDoorways = null;
	}

	public void DisableAllDoorways()
	{
		disableDoors = true;
	}

	private void Update()
	{
		RaycastHit hit;
		if (Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform.CompareTag("Doorway"))
		{
			Doorway doorway = hit.transform.gameObject.GetComponent<Doorway>();
			if (doorway == null)
			{
				throw new Exception("Doorway object has no doorway component attached!\n" + hit.transform.gameObject.ToString() + "\n" + hit.ToString());
			}

			if (currentDoorway != doorway)
			{
				if (currentDoorway != null)
				{
					currentDoorway.CloseDoor(true);
				}
				currentDoorway = doorway;

			}

			OVRGazePointer.instance.SetPosition(hit.point);

			if (doorway.openable && (!disableDoors || openableDoorways.Find(x=>x == hit.transform.gameObject)))
			{
				doorway.OpenDoor(keepOpenTime);
				GameManager.instance.RequestPointerEmphasis();

				if (OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(GameManager.instance.interactionKey))
				{
					doorway.ChangeRoom();
				}
			}
			else
			{
				GameManager.instance.RequestPointerEmphasis(true);
			}
		}
	}
}
