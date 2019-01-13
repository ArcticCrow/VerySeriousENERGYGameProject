using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour {

	public bool disableThrowing = false;

	public float throwForce = 800f;
	public Vector3 handOffset;

	public Transform itemInHand;

	Rigidbody itemRB;
	bool isHolding = false;

	private void Start()
	{
		if (itemInHand != null)
		{
			PickUpObject(itemInHand);
		}
	}

	void Update()
	{
		RaycastHit hit;
		// If we are holding something already, let the player thro it
		if (isHolding == true && (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetMouseButtonDown(0)))
		{
			ThrowObject();
		}
		// If we are not holding anything, check if the player is pointing at a throwable object
		else if (isHolding == false && Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform.CompareTag("Throwable"))
		{
			OVRGazePointer.instance.SetPosition(hit.point);
			if (!disableThrowing)
			{
				GameManager.instance.RequestPointerEmphasis();
				if ((OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(GameManager.instance.interactionKey)))
				{
					PickUpObject(hit.transform);
				}
			}
			else
			{
				GameManager.instance.RequestPointerEmphasis(true);
			}
		}


	}
	void PickUpObject(Transform throwable)
	{
		// We are now holding an item, so set the flag
		isHolding = true;

		// To further interact with this item, set up references
		itemInHand = throwable;
		itemRB = itemInHand.GetComponent<Rigidbody>();

		// If for soem reason the item does not have a rb, add one
		if (itemRB == null)
		{
			itemRB = itemInHand.gameObject.AddComponent<Rigidbody>();
		}

		// Put the item in the players hand (in front of the pointer) & make it move with the hand
		itemInHand.SetParent(GameManager.instance.Pointer);
		itemInHand.localPosition = handOffset;

		// We only want to move the item with the player, while it's being held
		itemRB.isKinematic = true;
	}

	void ThrowObject()
	{
		// We are throwing the item, so remove the flag
		isHolding = false;

		// Remove the object parent, and thus the movement with the parent
		itemInHand.transform.SetParent(null);

		// Enable physics add force an let the rest work out on it's own
		itemRB.isKinematic = false;

		// Apply throw force if allowed
		if (!disableThrowing)
			itemRB.AddForce(GameManager.instance.Pointer.forward * throwForce, ForceMode.Impulse);
	}
}
