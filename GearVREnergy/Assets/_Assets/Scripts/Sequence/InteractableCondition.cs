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
	public bool overrideStartValue;
	public bool startValue;


	public override bool IsConditionMet()
	{
		if (!isInitialized) return false;

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
		if (overrideStartValue) {
			interactable.isPowered = false;
		}
	}

	public override void Finish()
	{
		if (disableAIInteraction)
		{
			ShipAI.Instance.IgnoreInteractableInteraction(interactable.gameObject);
		}
	}
}
