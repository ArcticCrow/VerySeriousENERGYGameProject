using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInformation : MonoBehaviour {
	private static int nextID = 0;

	[HideInInspector]
	public int id = nextID++;

    [Header("General")]
	public string roomName = "Unnamed";
	public Color colorCode = Color.white;
	public Transform playerTeleportTransform;

    public bool isActived = true;

    [Header("Energy")]
    public float energyConsumptionPerSecond;
    public float energyGenerationPerSecond;

    public RoomEnergyLevel influenceLevel;

    public List<Interactable> interactables;

    [Header("Connections")]
	public List<Doorway> doorways;


	private void OnValidate()
	{
		roomName = name;
	}

	public void UpdateInfluence()
    {
		interactables = new List<Interactable>(gameObject.GetComponentsInChildren<Interactable>()) {};
		for (int i = 0; i < interactables.Count; i++ )
		{
			interactables[i].roomLocation = this;
		}

		UpdateEnergyLevel();
    }

	public void UpdateEnergyLevel()
	{
        energyGenerationPerSecond = 0;
		energyConsumptionPerSecond = 0;
		for (int i = 0; i < interactables.Count; i++)
		{
			if (interactables[i] != null && interactables[i].isActiveAndEnabled && interactables[i].gameObject.activeInHierarchy && interactables[i].isPowered)
			{
                float influenceAmount = 0;
                if (interactables[i].overrideInfluence)
                {
                    influenceAmount = interactables[i].overriddenInfluence;
                }
                else
                {
                    EnergyClass ec = EnergyManager.instance.GetEnergyClass(interactables[i].classLabel);
                    influenceAmount = ec.energyInfluencePerSecond;
                }

                if (influenceAmount > 0)
                {
                    energyGenerationPerSecond += influenceAmount;
                }
                else
                {
                    energyConsumptionPerSecond -= influenceAmount;
                }
			}
		}

		influenceLevel = EnergyManager.instance.GetInfluenceLevel(energyGenerationPerSecond + energyConsumptionPerSecond);
	}
}
