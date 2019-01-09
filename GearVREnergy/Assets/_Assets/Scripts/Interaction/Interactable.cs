using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour {

    public bool disableInteraction = false;

    public bool isPowered = false;
    public bool badPowerState;

	public bool needsStateUpdate = false;

	public EnergyLabel classLabel;

	public bool overrideInfluence = false;
	public float overriddenInfluence = 0;

    [SerializeField] private UnityEvent powerOnEvents;
	[SerializeField] private UnityEvent powerOffEvents;

	public RoomInformation roomLocation;

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
        if (Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform == transform)
        {
            GameManager.instance.RequestPointerEmphasis();
			OVRGazePointer.instance.SetPosition(hit.point);

			if (!disableInteraction && (OVRInput.GetUp(GameManager.instance.interactionButton) || Input.GetKeyUp(GameManager.instance.interactionKey)))
			{
				isPowered = !isPowered;
				needsStateUpdate = true;
			}
        }
		
		if (needsStateUpdate)
		{
			CallPowerEvents();
			EnergyManager.Instance.needsUpdate = true;
			if (roomLocation != null)
			{
				roomLocation.UpdateEnergyLevel();
			}
			needsStateUpdate = false;
		}
	}

	public void ActivateBadPowerState(bool activate)
	{
		isPowered = (activate)? badPowerState : !badPowerState;
		needsStateUpdate = true;
	}

}
