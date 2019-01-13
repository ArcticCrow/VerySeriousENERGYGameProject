using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomCondition : Condition
{
    public enum CheckType
    {
        isActiveRoom,
        isEnergyLevel,
    }
    public CheckType check;
    public RoomInformation room;
    public RoomEnergyLabel energyLabel;
    public bool targetValue;

	public override bool IsConditionMet()
    {
		if (!isInitialized) return false;
        if (room == null)
            throw new Exception("Room condition needs room information component to check condition!");
        switch (check)
        {
            case CheckType.isActiveRoom:
                return (room == GameManager.instance.currentRoom) == targetValue;

            case CheckType.isEnergyLevel:
                return (room.energyLevel.label == energyLabel) == targetValue;
        }

        Debug.LogAssertion("Something went wrong when checking a room condition");
        return false;
    }

	public override void Initialize()
	{
		isInitialized = true;
	}

	public override void Finish()
	{
		
	}
}
