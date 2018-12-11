using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour {

	[Header("General")]
	public float maxEnergy = 10000f; //perhaps we should you real life values?
	public float startingEnergy = 1000f;
	public float currentEnergy;

	[Header("Energy Influences")]
	[Tooltip("The amount of energy that is regenerated over each interval.")]
	public float baseEnergyGenerationPerSecond = 500f;
	[Range(1f, 100f), Tooltip("How often the current energy level changes. (Ticks per Second)")]
	public float energyTicksPerSecond = 10f;

	[SerializeField, Tooltip("A list of all objects that the manager should consider for controlling the energy levels.")]
	private List<string> energyInfluencingObjectTags = new List<string>()
	{
		"Interactable"
	};
	[SerializeField, Tooltip("Listing of all found consumer objects in the scene. Will be generated on scene start.")]
	private List<GameObject> energyInfluencingObjects;

	[Header("Energy Statistics")]
	[SerializeField, Tooltip("")]
	private float energyConsumptionPerTick = 0;
	[SerializeField, Tooltip("")]
	private float energyGenerationPerTick = 0;
	[SerializeField, Tooltip("")]
	private float fluctuationPerTick = 0;
	[SerializeField, Tooltip("")]
	private float fluctuationPerSecond = 0;

	[Header("Routine Control")]
	[SerializeField]
	bool startEnergyRoutine = true;
	[SerializeField]
	bool stopEnergyRoutine = false;
	[SerializeField]
	bool isEnergyRoutineRunning = false;


	// Use this for initialization
	void Start () {
		currentEnergy = startingEnergy;

		CreateObjectListing();
		CalculateEnergyFluctuation();
	}

	private IEnumerator ControlEnergyLevel()
	{
		isEnergyRoutineRunning = true;

		while (isEnergyRoutineRunning)
		{
			yield return new WaitForSeconds(1f/energyTicksPerSecond);

			//print("Energy level:" + currentEnergy);

			if (stopEnergyRoutine)
			{
				print("Energy Coroutine is stopping ...");
				isEnergyRoutineRunning = false;
				stopEnergyRoutine = false;
			}
			else
			{
				CalculateEnergyFluctuation();
				currentEnergy = Mathf.Clamp(currentEnergy + fluctuationPerTick, 0, maxEnergy);
			}
		}
	}

	private void CalculateEnergyFluctuation()
	{
		energyConsumptionPerTick = 0;
		energyGenerationPerTick = 0;

		CheckEnergyInfluencingObjects();

		energyGenerationPerTick += baseEnergyGenerationPerSecond / energyTicksPerSecond;

		fluctuationPerTick = energyGenerationPerTick - energyConsumptionPerTick;
		fluctuationPerSecond = fluctuationPerTick * energyTicksPerSecond;
	}

	private void CheckEnergyInfluencingObjects()
	{
		for (int i = 0; i < energyInfluencingObjects.Count; i++)
		{
			GameObject obj = energyInfluencingObjects[i];
			// Here I would get the energy component and check if the device is on or off
			// Placeholder wise I just assume that every object with a certain color is on or off
			MeshRenderer mr = obj.GetComponent<MeshRenderer>();

			Color currentColor = mr.material.color;
			Color onColor = Color.black;

			// I'm assuming that the object is on and thus consuming power
			// Really I should be checking if a device is powered on or off and if it is 
			// consuming or generating power
			if (currentColor == onColor)
			{
				energyConsumptionPerTick += 100f / energyTicksPerSecond;
			}
		}
	}

	private void CreateObjectListing()
	{
		energyInfluencingObjects = new List<GameObject>();
		for (int i = 0; i < energyInfluencingObjectTags.Count; i++)
		{
			string tag = energyInfluencingObjectTags[i];
			energyInfluencingObjects.AddRange(GameObject.FindGameObjectsWithTag(tag));
		}
		if (energyInfluencingObjects.Count == 0)
		{
			throw new Exception("No energy influencers could be found in the scene!");
		}
	}

	void Update()
	{
		if (startEnergyRoutine)
		{
			if (!isEnergyRoutineRunning)
			{
				print("Energy Coroutine is starting ...");
				// Start AI coroutine
				StartCoroutine(ControlEnergyLevel());
			}
			else
			{
				Debug.LogWarning("Energy Coroutine is still running!");
			}
			startEnergyRoutine = false;
		}
	}

}
