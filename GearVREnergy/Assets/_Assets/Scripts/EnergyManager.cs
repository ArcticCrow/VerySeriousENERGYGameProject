using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnergyLabel
{
	None,
	Low,
	Moderate,
	High,
}

[Serializable]
public struct EnergyClass
{
	public EnergyLabel label;
	public Color color;
	public float energyInfluence;

	public EnergyClass(EnergyLabel label, Color color, float energyInfluence)
	{
		this.label = label;
		this.color = color;
		this.energyInfluence = energyInfluence;
	}
}

public class EnergyManager : MonoBehaviour {

	[HideInInspector]
	public static EnergyManager instance;

	[Header("General")]
	public float energyCapacity = 10000f; //perhaps we should you real life values?
	public float startingEnergy = 1000f;
	public float currentEnergy;

	[Header("Energy Influences")]
	[Tooltip("The base amount of energy change that happens over each interval.")]
	public float baseEnergyFluctuationPerSecond = 0;
	[Range(1f, 100f), Tooltip("How often the current energy level changes. (Ticks per Second)")]
	public float ticksPerSecond = 10f;

	public List<EnergyClass> energyClasses = new List<EnergyClass>()
	{
		new EnergyClass(EnergyLabel.None, Color.blue, 0),
		new EnergyClass(EnergyLabel.Low, Color.green, 10),
		new EnergyClass(EnergyLabel.Moderate, Color.yellow, 25),
		new EnergyClass(EnergyLabel.High, Color.red, 100)
	};

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


	public bool needsUpdate = false;

	// Use this for initialization
	void Start () {
		CheckSingeltonInstance();

		currentEnergy = startingEnergy;

		CreateObjectListing();
		CalculateEnergyFluctuation();
	}

	private void CheckSingeltonInstance()
	{
		//if (instance != null) Destroy(gameObject);
		instance = this;
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
				currentEnergy = Mathf.Clamp(currentEnergy + fluctuationPerTick, 0, energyCapacity);
			}
		}
	}

	private void CalculateEnergyFluctuation()
	{
		CheckEnergyInfluencesObjects();

		// Calculate totals
		fluctuationPerTick = positiveInfluencePerTick - negativeInfluencePerTick;
		fluctuationPerSecond = fluctuationPerTick * ticksPerSecond;
	}

	private void CheckEnergyInfluencesObjects()
	{
		if (needsUpdate)
		{
			needsUpdate = false;

			negativeInfluencePerTick = 0;
			positiveInfluencePerTick = 0;

			// Iterate over a list of all energy influencing objects that need to be considered
			for (int i = 0; i < energyInfluencingObjects.Count; i++)
			{
				Interactable energyInfluence = energyInfluencingObjects[i].GetComponent<Interactable>();
				// Interactable script is present, it's gameobject is in the scene and it's powered on
				if (energyInfluence != null && energyInfluence.isActiveAndEnabled && energyInfluence.gameObject.activeInHierarchy && energyInfluence.isPowered)
				{
					float influenceAmount = 0;
					if (energyInfluence.overrideInfluence)
					{
						influenceAmount = energyInfluence.overriddenInfluence;
					}
					else
					{
						influenceAmount = GetEnergyClass(energyInfluence.classLabel).energyInfluence;
					}

					if (influenceAmount < 0)
					{
						positiveInfluencePerTick -= influenceAmount;
					}
					else
					{
						negativeInfluencePerTick += influenceAmount;
					}
				}
			}

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
		}
	}

	public EnergyClass GetEnergyClass(EnergyLabel label)
	{
		return energyClasses.Find(x => x.label == label);
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
