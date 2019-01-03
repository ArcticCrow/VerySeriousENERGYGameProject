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

	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		
		if (Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform == transform)
		{
			if (!openable)
			{
				// show special gaze pointer
			}
			else
			{
                GameManager.instance.RequestPointerEmphasis();
				OVRGazePointer.instance.SetPosition(hit.point);
				if (!open)
				{
					open = true;
					if (door != null)
					{
                        SoundControl.PlaySound(SFXClip.Door);
                        door.GetComponent<Animator>().SetBool("LookAt", true);
					}
					ToggleHighlightColor(true);
				}
				
				
				if ((OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(GameManager.instance.interactionKey)))
				{
                    GameManager.instance.TeleportToOtherRoom(roomA, roomB);
				}
			}
		}
		else if (open)
		{
			open = false;

			if (door != null)
			{
				door.GetComponent<Animator>().SetBool("LookAt", false);
			}
			ToggleHighlightColor(false);
		}
	}
}
