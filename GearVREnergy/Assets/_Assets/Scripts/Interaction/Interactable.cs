﻿using System.Collections;
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

	public void TogglePower()
	{
		SetPowerState(!isPowered);
	}

	public void SetPowerState(bool activate)
	{
		isPowered = activate;
		needsStateUpdate = true;
	}
	public void EnableInteraction(bool enableInteraction)
	{
		disableInteraction = !enableInteraction;
	}

	public void ActivateBadPowerState(bool activate)
	{
		isPowered = (activate) ? badPowerState : !badPowerState;
		needsStateUpdate = true;
	}

	void Update()
    {
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

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Throwable"))
		{
			if (!disableInteraction && GameManager.instance.interactionControl.IsInteractionAllowedWith(gameObject))
			{
				TogglePower();
			}
		}
	}
}
