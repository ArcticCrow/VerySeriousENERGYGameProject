using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRoom : MonoBehaviour {

	public bool openable = true;

	public TeleportPoint destination;
	public TeleportPoint current;

	public bool animateDoor = true;

	public GameObject door;

	// Use this for initialization
	void Start () {
		if (door == null)
		{
			door = gameObject.GetComponentInChildren<Animator>().gameObject;
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
				door.GetComponent<Animator>().SetBool("LookAt", true);

				if (OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(KeyCode.A))
				{
					print("change room");
				}
			}
		}
		else
		{
			door.GetComponent<Animator>().SetBool("LookAt", false);
		}
	}
}
