using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour {

    public bool isPowered = false;
    public bool badPowerState;

	public bool needsStateUpdate = false;

    public float energyInfluencePerSecond = 50f;

    [SerializeField] private UnityEvent powerOnEvents;
	[SerializeField] private UnityEvent powerOffEvents;

	void Start ()
	{
	    CallPowerEvents();
	}	
	private void CallPowerEvents()
	{
		if(isPowered)
		{
			print(gameObject.name + ": Interactable: Powering on");
			powerOnEvents.Invoke();
		}
		else
		{
			print(gameObject.name + ": Interactable: Powering off");
			powerOffEvents.Invoke();
		}
	}

    void Update()
    {
        RaycastHit hit; 
        if (Physics.Raycast(GameManager.instance.pointerTransform.position, GameManager.instance.pointerTransform.forward, out hit, GameManager.instance.maxInteractionRange, GameManager.instance.interactionMask))
        {
            OVRGazePointer.instance.RequestShow();

			if (hit.transform == transform)
			{
				if (OVRInput.GetUp(GameManager.instance.interactionButton) || Input.GetKeyUp(GameManager.instance.interactionKey) || Input.GetMouseButtonUp(0))
				{
					isPowered = !isPowered;
					needsStateUpdate = true;
				}
			}
        }
		
		if (needsStateUpdate)
		{
			CallPowerEvents();
			needsStateUpdate = false;
		}
	}

	public void ActivateBadPowerState(bool activate)
	{
		isPowered = (activate)? badPowerState : !badPowerState;
		needsStateUpdate = true;
	}

}
