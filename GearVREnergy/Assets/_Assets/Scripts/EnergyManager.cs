using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
	public float energyInfluencePerSecond;

	public EnergyClass(EnergyLabel label, Color color, float energyInfluencePerSecond)
	{
		this.label = label;
		this.color = color;
		this.energyInfluencePerSecond = energyInfluencePerSecond;
	}
}

public enum RoomEnergyLabel
{
    None,
    Low,
    Moderate,
    High,
    Extreme,
}

[Serializable]
public struct RoomEnergyLevel
{
    public RoomEnergyLabel label;
    public Color warningColor;
    public int minConsumption;
    public int maxConsumption;

    public RoomEnergyLevel(RoomEnergyLabel label, Color warningColor, int minConsumption, int maxConsumption)
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

public class EnergyManager : MonoBehaviour {

	[HideInInspector]
	private static EnergyManager instance;
	public static EnergyManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameManager.instance.gameObject.GetComponent<EnergyManager>();
				if (instance == null)
				{
					instance = GameManager.instance.gameObject.AddComponent<EnergyManager>();
				}
			}

			return instance;
		}
	}

	[Header("General")]
	public float energyUsedThisRun = 0;
	public List<float> energyConsumedDuringJourney = new List<float>();

	public float totalEnergyUsedThisFlight = 0;

    public UnityEvent energyDependantEvents;
    public UnityEvent newRunEvents;
	[Range(0.01f, 2f)]
	public float multiplier = 1f;

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

	[Tooltip("A list of all objects that the manager should consider for controlling the energy levels.")]
	public List<string> energyInfluencingObjectTags = new List<string>()
	{
		"Interactable"
	};

    public List<RoomEnergyLevel> consumptionLevels = new List<RoomEnergyLevel>()
    {
        new RoomEnergyLevel(RoomEnergyLabel.None, Color.blue, -1, 0),
        new RoomEnergyLevel(RoomEnergyLabel.Low, Color.green, 1, 10),
        new RoomEnergyLevel(RoomEnergyLabel.Moderate, Color.yellow, 11, 35),
        new RoomEnergyLevel(RoomEnergyLabel.High, Color.yellow + Color.red, 36, 100),
        new RoomEnergyLevel(RoomEnergyLabel.Extreme, Color.red, 101, -1),
    };

    [Header("Energy Statistics")]
	[SerializeField, Tooltip("")]
    private float negativeInfluencePerTick = 0;
	[SerializeField, Tooltip("")]
    private float positiveInfluencePerTick = 0;
    [SerializeField, Tooltip("")]
    private float fluctuationPerTick = 0;

	[SerializeField, Tooltip("")]
    private float positiveInfluencePerSecond = 0;
    [SerializeField, Tooltip("")]
    private float negativeInfluencePerSecond = 0;
	[SerializeField, Tooltip("")]
	private float fluctuationPerSecond = 0;

	[Header("Routine Control")]
	public bool startEnergyRoutine = true;
	public bool stopEnergyRoutine = false;
	[SerializeField]
	bool isEnergyRoutineRunning = false;

	public bool needsUpdate = false;

    bool isInitialized = false;

    private void Start()
    {
        CheckSingeltonInstance();
    }

    public void Initialize () {
        needsUpdate = true;

        CalculateEnergyFluctuation();
        isInitialized = true;

    }

    private void InvokeEnergyDependances()
    {
		if (energyDependantEvents != null)
			energyDependantEvents.Invoke();
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
            yield return new WaitForSeconds(1f / ticksPerSecond);

			if (stopEnergyRoutine)
			{
				print("Energy Coroutine is stopping ...");
				isEnergyRoutineRunning = false;
				stopEnergyRoutine = false;
			}
			else
			{
				CalculateEnergyFluctuation();
				ApplyCurrentFluctuation();
			}
		}
	}

	private void ApplyCurrentFluctuation()
	{
		energyUsedThisRun = Mathf.Clamp(energyUsedThisRun + fluctuationPerTick * multiplier, 0, GameManager.instance.maxAmountOfEnergyPerRun);

		UpdateCurrentEnergyInJourney();
	}

	private void UpdateCurrentEnergyInJourney()
	{
		if (energyConsumedDuringJourney == null)
		{
			energyConsumedDuringJourney = new List<float>();
		}
		if (energyConsumedDuringJourney.Count < GameManager.instance.numberOfCurrentRun)
		{
			energyConsumedDuringJourney.Add(energyUsedThisRun);
			if (newRunEvents != null) newRunEvents.Invoke();
		}
		else
		{
			energyConsumedDuringJourney[GameManager.instance.numberOfCurrentRun - 1] = energyUsedThisRun;
		}
	}

	public void SetMultiplier(float val)
	{
		multiplier = val;
	}

	public void ResetEnergyUsedThisRun()
	{
		energyUsedThisRun = 0;
		UpdateCurrentEnergyInJourney();
	}

	private void CalculateEnergyFluctuation()
	{
		CheckEnergyInfluences();

		// Calculate totals
		fluctuationPerTick = positiveInfluencePerTick - negativeInfluencePerTick;
		fluctuationPerSecond = positiveInfluencePerSecond - negativeInfluencePerSecond;
	}

	private void CheckEnergyInfluences()
	{
        if (needsUpdate)
		{
            if (GameManager.instance.rooms.Count == 0)
            {
                throw new Exception("No rooms (with information) were given! Energy influence cannot be caluclated.");
            }

            needsUpdate = false;

			float positiveInfluencePerSecond = 0;
            float negativeInfluencePerSecond = 0;

			// Iterate over a list of all GameManager.instance.rooms with possible energy fluctuatuions, update them and add their influences to the statistics
			for (int i = 0; i < GameManager.instance.rooms.Count; i++)
			{
                GameManager.instance.rooms[i].UpdateInfluence();
                positiveInfluencePerSecond += GameManager.instance.rooms[i].energyGenerationPerSecond;
                negativeInfluencePerSecond += GameManager.instance.rooms[i].energyConsumptionPerSecond;
            }

			if (baseEnergyFluctuationPerSecond > 0)
			{
                positiveInfluencePerSecond += baseEnergyFluctuationPerSecond;
			}
			else
			{
                negativeInfluencePerSecond += baseEnergyFluctuationPerSecond;
			}

			// Apply tick scale
			positiveInfluencePerTick = positiveInfluencePerSecond / ticksPerSecond;
			negativeInfluencePerTick = negativeInfluencePerSecond / ticksPerSecond;

            InvokeEnergyDependances();
        }
    }

	public EnergyClass GetEnergyClass(EnergyLabel label)
	{
		return energyClasses.Find(x => x.label == label);
	}

    public RoomEnergyLevel GetInfluenceLevel(float consumption)
    {
        for (int i = 0; i < consumptionLevels.Count; i++)
        {
            RoomEnergyLevel level = consumptionLevels[i];
            if (level.CheckEnergyConsumption(consumption))
            {
                return level;
            }
        }
        throw new Exception("Consupmtion Level for " + consumption + " not found!");
    }

	void Update()
	{
        if (startEnergyRoutine)
        {
            if (!isInitialized) Initialize();
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
        if (!isEnergyRoutineRunning)
        {
            CalculateEnergyFluctuation();
        }
	}
}
