using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInformation : MonoBehaviour {
	private static int nextID = 0;

	[HideInInspector]
	public int id = nextID++;

	public string roomName = "Unnamed";
	public Color colorCode = Color.white;
	public Transform playerTeleportTransform;

	public float energyConsumption;
	public List<Interactable> interactables;

	public List<Doorway> doorways;

	public RoomEnergyConsumptionLevel consumptionLevel;

	public bool energyLevelUpdate = false;

	private void OnValidate()
	{
		roomName = name;
	}

	private void Start()
    {
		interactables = new List<Interactable>(gameObject.GetComponentsInChildren<Interactable>()) {};
		for (int i = 0; i < interactables.Count; i++ )
		{
			interactables[i].roomLocation = this;
		}

		UpdateEnergyLevel();
    }

	internal void UpdateEnergyLevel()
	{
		energyConsumption = 0;
		for (int i = 0; i < interactables.Count; i++)
		{
			if (interactables[i] != null && interactables[i].isActiveAndEnabled && interactables[i].gameObject.activeInHierarchy && interactables[i].isPowered)
			{
				EnergyClass ec = EnergyManager.instance.GetEnergyClass(interactables[i].classLabel);
				energyConsumption += ec.energyInfluence;
			}
		}

		consumptionLevel = ShipManager.instance.GetConsumptionLevel(energyConsumption);
		energyLevelUpdate = true;
	}
}
