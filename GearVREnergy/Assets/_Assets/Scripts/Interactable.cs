using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour {

    public bool isPowered = false;
    public bool badPowerState;

    public float energyConsumption = 50f;

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
			powerOnEvents.Invoke();
		}
		else
		{
			powerOffEvents.Invoke();
		}
	}

    void Update()
    {
        RaycastHit hit; 
        if (Physics.Raycast(GameManager.instance.pointerTransform.position, GameManager.instance.pointerTransform.forward, out hit, GameManager.instance.maxInteractionRange, GameManager.instance.interactionMask))
        {
            OVRGazePointer.instance.RequestShow();
            if (OVRInput.GetUp(GameManager.instance.interactionButton) || Input.GetKeyUp(GameManager.instance.interactionKey) || Input.GetMouseButtonUp(0))
            {
                isPowered = !isPowered;
                CallPowerEvents();
            }
        }
		
	}

}
