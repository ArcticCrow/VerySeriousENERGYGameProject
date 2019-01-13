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

	public void DisableAllInteractions()
	{
		disableInteraction = true;
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

			if (!interactable.disableInteraction && (!disableInteraction || allowedInteractions.Find(x => x == hit.transform.gameObject) != null))
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

