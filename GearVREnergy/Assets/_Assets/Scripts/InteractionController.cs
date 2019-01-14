using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {
	public bool disableInteraction = false;
	[SerializeField]
	List<GameObject> allowedInteractions;

	public void EnableInteractionWith(GameObject _gameObject)
	{
		if (allowedInteractions == null)
		{
			allowedInteractions = new List<GameObject>();
		}
		allowedInteractions.Add(_gameObject);
	}

	public void DisableInteractionWith(GameObject _gameObject)
	{
		if (allowedInteractions == null) return;
		allowedInteractions.RemoveAll(x => x == _gameObject);
	}

	public void EnableAllInteractions(bool resetList = false)
	{
		disableInteraction = false;
		if (resetList) allowedInteractions = null;
	}

	public void DisableAllInteractions(bool resetList = false)
	{
		disableInteraction = true;
		if (resetList) allowedInteractions = null;
	}

	public void EnableInteractionForRoom(RoomInformation room)
	{
		if (allowedInteractions == null)
		{
			allowedInteractions = new List<GameObject>();
		}
		for (int i = 0; i < room.interactables.Count; i++)
		{
			allowedInteractions.Add(room.interactables[i].gameObject);
		}
	}

	public void DisableInteractionForRoom(RoomInformation room)
	{
		if (allowedInteractions == null) return;
		for (int i = 0; i < room.interactables.Count; i++)
		{
			allowedInteractions.RemoveAll(x => x == room.interactables[i].gameObject);
		}
	}

	public bool IsInteractionAllowedWith(GameObject _gameObject)
	{
		/*string temp = "IsInteractionAllowedWith " + _gameObject.name + ": " + (allowedInteractions.Find(x => x == _gameObject) != null);
		for (int i = 0; i < allowedInteractions.Count; i++)
		{
			temp += "\nAllowed: " + allowedInteractions[i].name;
		}
		print(temp);*/

		return !disableInteraction || (allowedInteractions != null && allowedInteractions.Find(x => x == _gameObject) != null);
	}

	private void Update()
	{
		RaycastHit hit; 
        if (Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform.CompareTag("Interactable"))
        {
			Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
			if (interactable == null)
			{
				throw new Exception("No interactable component attached to interactable!\n" + hit.transform.gameObject.ToString() + "\n" + hit.ToString());
			}

			OVRGazePointer.instance.SetPosition(hit.point);

			if (!interactable.disableInteraction && IsInteractionAllowedWith(hit.transform.gameObject))
			{
				GameManager.instance.RequestPointerEmphasis();
				if (OVRInput.GetUp(GameManager.instance.interactionButton) || Input.GetKeyUp(GameManager.instance.interactionKey))
				{
					interactable.TogglePower();
				}
			}
			else
			{
				GameManager.instance.RequestPointerEmphasis(true);
			}
        }
	}
}

