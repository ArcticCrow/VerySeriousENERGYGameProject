using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {
	public bool disableInteraction = false;

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

			if (!disableInteraction && !interactable.disableInteraction)
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

