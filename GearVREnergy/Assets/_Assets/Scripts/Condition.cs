using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class Condition : MonoBehaviour {
	public abstract bool IsConditionMet();
}

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

	public override bool IsConditionMet()
	{
		if (interactable == null)
			throw new Exception("Interactable condition needs interactable component to check condition!\nGiven: " + this.interactable.GetType().ToString());

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
}
