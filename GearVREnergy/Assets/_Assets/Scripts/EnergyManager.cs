using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour {

	[HideInInspector]
	public static EnergyManager instance;

	[Header("General")]
	public float maxEnergy = 10000f; //perhaps we should you real life values?
	public float startingEnergy = 1000f;
	public float currentEnergy;

	[Header("Energy Influences")]
	[Tooltip("The base amount of energy change that happens over each interval.")]
	public float baseEnergyFluctuationPerSecond = 0;
	[Range(1f, 100f), Tooltip("How often the current energy level changes. (Ticks per Second)")]
	public float ticksPerSecond = 10f;

	[SerializeField, Tooltip("A list of all objects that the manager should consider for controlling the energy levels.")]
	private List<string> energyInfluencingObjectTags = new List<string>()
	{
		"Interactable"
	};
	[SerializeField, Tooltip("Listing of all found consumer objects in the scene. Will be generated on scene start.")]
	private List<GameObject> energyInfluencingObjects;

	[Header("Energy Statistics")]
	[Tooltip("")]
	public float negativeInfluencePerTick = 0;
	[Tooltip("")]
	public float positiveInfluencePerTick = 0;
	[SerializeField, Tooltip("")]
	private float fluctuationPerTick = 0;
	[SerializeField, Tooltip("")]
	private float fluctuationPerSecond = 0;

	[Header("Routine Control")]
	public bool startEnergyRoutine = true;
	public bool stopEnergyRoutine = false;
	[SerializeField]
	bool isEnergyRoutineRunning = false;


	// Use this for initialization
	void Start () {
		CheckSingeltonInstance();

		currentEnergy = startingEnergy;

		CreateObjectListing();
		CalculateEnergyFluctuation();
	}

	private void CheckSingeltonInstance()
	{
		if (instance != null)
			Destroy(gameObject);

		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private IEnumerator ControlEnergyLevel()
	{
		isEnergyRoutineRunning = true;

		while (isEnergyRoutineRunning)
		{
			yield return new WaitForSeconds(1f/ticksPerSecond);

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
		negativeInfluencePerTick = 0;
		positiveInfluencePerTick = 0;

		CheckEnergyInfluencingObjects();

		if (baseEnergyFluctuationPerSecond > 0)
		{
			positiveInfluencePerTick += baseEnergyFluctuationPerSecond;
		}
		else
		{
			negativeInfluencePerTick += baseEnergyFluctuationPerSecond;
		}

		// Apply tick scale
		positiveInfluencePerTick /= ticksPerSecond;
		negativeInfluencePerTick /= ticksPerSecond;

		// Calculate totals
		fluctuationPerTick = positiveInfluencePerTick - negativeInfluencePerTick;
		fluctuationPerSecond = fluctuationPerTick * ticksPerSecond;
	}

	private void CheckEnergyInfluencingObjects()
	{
		// Iterate over a list of all energy influencing objects that need to be considered
		for (int i = 0; i < energyInfluencingObjects.Count; i++)
		{
			Interactable energyInfluence = energyInfluencingObjects[i].GetComponent<Interactable>();
			// Interactable script is present, it's gameobject is in the scene and it's powered on
			if (energyInfluence != null && energyInfluence.isActiveAndEnabled && energyInfluence.gameObject.activeInHierarchy && energyInfluence.isPowered)
			{
				if (energyInfluence.energyInfluencePerSecond > 0)
				{
					positiveInfluencePerTick += energyInfluence.energyInfluencePerSecond;
				} 
				else
				{
					negativeInfluencePerTick -= energyInfluence.energyInfluencePerSecond;
				}
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
