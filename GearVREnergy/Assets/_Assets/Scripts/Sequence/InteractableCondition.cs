using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InteractableCondition : Condition
{
	public enum CheckType
	{
		isPowered,
		isBadPowerState,
	}
	public Interactable interactable;
	public CheckType check;
	public bool targetValue;

	[Header("Interactable Control")]
	public bool disableAIInteraction = false;
	public bool disableOtherInteractables = false;
	public bool overrideStartValue;
	public bool startValue;


	bool disabledInteractionState;

	public override bool IsConditionMet()
	{
		if (!isInitialized)
		{
			print("not initialized");
			return false;
		}

		if (interactable == null)
			throw new Exception("Interactable condition needs interactable component to check condition!");

		switch (check)
		{
			case CheckType.isPowered:
				return interactable.isPowered == targetValue;

			case CheckType.isBadPowerState:
				return (interactable.isPowered == interactable.badPowerState) == targetValue;
		}

		Debug.LogAssertion("Something went wrong when checking a condition");
		return false;
	}

	public override void Initialize()
	{
		if (disableAIInteraction)
		{
			ShipAI.Instance.IgnoreInteractableInteraction(interactable.gameObject);
		}
		if (disableOtherInteractables)
		{
			disabledInteractionState = GameManager.instance.interactionControl.disableInteraction;
			if (!disabledInteractionState)
			{
				GameManager.instance.interactionControl.DisableAllInteractions();
			}
			GameManager.instance.interactionControl.EnableInteractionWith(interactable.gameObject);
		}
		if (overrideStartValue) {
			interactable.SetPowerState(startValue);
		}

		interactable.EnableInteraction(true);
		isInitialized = true;
	}

	public override void Finish()
	{
		if (disableAIInteraction)
		{
			ShipAI.Instance.IgnoreInteractableInteraction(interactable.gameObject);
		}
		if (disableOtherInteractables)
		{
			if (!disabledInteractionState)
			{
				GameManager.instance.interactionControl.EnableAllInteractions(true);
			}
			GameManager.instance.interactionControl.DisableInteractionWith(interactable.gameObject);
		}
	}
}
