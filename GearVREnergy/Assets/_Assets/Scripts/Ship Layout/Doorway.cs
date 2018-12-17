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

	public float fadeSpeed = 1f;
	public float fadeLength = 0.5f;

	private bool teleporting;

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
		
		if (Physics.Raycast(GameManager.instance.pointerTransform.position, GameManager.instance.pointerTransform.forward, out hit) && hit.transform == transform)
		{
			if (!openable)
			{
				// show special gaze pointer
			}
			else
			{
				OVRGazePointer.instance.RequestShow();
				OVRGazePointer.instance.SetPosition(hit.point);
				if (!open)
				{
					open = true;
					if (door != null)
					{
						door.GetComponent<Animator>().SetBool("LookAt", true);
					}
					ToggleHighlightColor(true);
				}
				
				
				if (!teleporting && (OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(GameManager.instance.interactionKey)))
				{
					StartChangingRoom();
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

	private void StartChangingRoom()
	{
		teleporting = true;

		if (ShipManager.instance.currentRoom == roomA)
		{
			ShipManager.instance.SwitchCurrentRoom(roomB);
		}
		else if(ShipManager.instance.currentRoom == roomB)
		{
			ShipManager.instance.SwitchCurrentRoom(roomA);
		}
		else
		{
			throw new Exception("Player is trying to teleport out of a room, he is not in!");
		}

		Transform destTransform = ShipManager.instance.currentRoom.playerTeleportTransform;
		StartCoroutine(TeleportCoroutine(destTransform));
	}


	IEnumerator TeleportCoroutine(Transform destTransform)
	{
		Vector3 destPosition = destTransform.position;
		Quaternion destRotation = destTransform.rotation;

		float fadeLevel = 0;

		while (fadeLevel < 1)
		{
			yield return null;
			fadeLevel += fadeSpeed * Time.deltaTime;
			fadeLevel = Mathf.Clamp01(fadeLevel);
			OVRInspector.instance.fader.SetFadeLevel(fadeLevel);
		}

		GameManager.instance.player.transform.position = destPosition;

		Quaternion headRotation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
		Vector3 euler = headRotation.eulerAngles;
		euler.x = 0;
		euler.z = 0;
		headRotation = Quaternion.Euler(euler);
		GameManager.instance.player.transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(headRotation), 1) * destRotation;

		yield return new WaitForSeconds(fadeLength);

		teleporting = false;

		while (fadeLevel > 0)
		{
			yield return null;
			fadeLevel -= fadeSpeed * Time.deltaTime;
			fadeLevel = Mathf.Clamp01(fadeLevel);
			OVRInspector.instance.fader.SetFadeLevel(fadeLevel);
		}

		yield return null;
	}
}
