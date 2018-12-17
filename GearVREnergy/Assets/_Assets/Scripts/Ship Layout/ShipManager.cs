using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomEnergyLabel
{
	None,
	Low,
	Moderate,
	High,
	Extreme,
}

[Serializable]
public struct RoomEnergyConsumptionLevel
{
	public RoomEnergyLabel label;
	public Color warningColor;
	public int minConsumption;
	public int maxConsumption;

	public RoomEnergyConsumptionLevel(RoomEnergyLabel label, Color warningColor, int minConsumption, int maxConsumption)
	{
		this.label = label;
		this.warningColor = warningColor;
		this.minConsumption = minConsumption;
		this.maxConsumption = maxConsumption;
	}

	public bool CheckEnergyConsumption(float currentConsumption)
	{
		currentConsumption = Mathf.Round(currentConsumption);
		if (minConsumption == -1)
		{
			return currentConsumption <= maxConsumption;
		}
		else if (maxConsumption == -1)
		{
			return currentConsumption >= minConsumption;
		}
		else
		{
			return currentConsumption <= maxConsumption && currentConsumption >= minConsumption;
		}
	}
}

public class ShipManager : MonoBehaviour
{
	public static ShipManager instance;

	public RoomInformation currentRoom;
	public List<RoomInformation> rooms = new List<RoomInformation>();

	public List<RoomEnergyConsumptionLevel> consumptionLevels = new List<RoomEnergyConsumptionLevel>()
	{
		new RoomEnergyConsumptionLevel(RoomEnergyLabel.None, Color.blue, -1, 0),
		new RoomEnergyConsumptionLevel(RoomEnergyLabel.Low, Color.green, 1, 10),
		new RoomEnergyConsumptionLevel(RoomEnergyLabel.Moderate, Color.yellow, 11, 35),
		new RoomEnergyConsumptionLevel(RoomEnergyLabel.High, Color.yellow + Color.red, 36, 100),
		new RoomEnergyConsumptionLevel(RoomEnergyLabel.Extreme, Color.red, 101, -1),
	};

	private void Start()
	{
		CheckSingeltonInstance();
		rooms = new List<RoomInformation>(gameObject.GetComponentsInChildren<RoomInformation>());
	}

	private void CheckSingeltonInstance()
	{
		//if (instance != null) Destroy(gameObject);

		instance = this;
	}

	public void SwitchCurrentRoom(RoomInformation newActiveRoom)
	{
		// Disable interactables in other room

		// Enable interactables in this room

		currentRoom = newActiveRoom;
	}

	public RoomEnergyConsumptionLevel GetConsumptionLevel(float consumption)
	{
		for (int i = 0; i < consumptionLevels.Count; i++)
		{
			RoomEnergyConsumptionLevel level = consumptionLevels[i];
			if (level.CheckEnergyConsumption(consumption))
			{
				return level;
			}
		}
		throw new Exception("Consupmtion Level for " + consumption + " not found!");
	}
}
