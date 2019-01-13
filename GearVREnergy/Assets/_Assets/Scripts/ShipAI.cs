using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ShipAI : MonoBehaviour {

	[HideInInspector]
	private static ShipAI instance;
	public static ShipAI Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameManager.instance.gameObject.GetComponent<ShipAI>();
				if (instance == null)
				{
					instance = GameManager.instance.gameObject.AddComponent<ShipAI>();
				}
			}

			return instance;
		}
	}

	[Header("General")]
	[SerializeField, Tooltip("A list of all objects that the AI should look to man  ipulate.")]
	private List<string> interactableObjectTags = new List<string>()
	{
		"Interactable"
	};
	[SerializeField, Tooltip("Listing of all found interactable objects in the scene. Will be generated on scene start.")]
	List<GameObject> interactables;

	List<GameObject> ignoredInteractables;

	[Header("Interval Control")]
	[Tooltip("The minimum time difference, before the AI makes its next move.")]
	public float minTimeDifference = 5f;
	[Tooltip("The maximum time difference, before the AI makes its next move.")]
	public float maxTimeDifference = 10f;
	[Tooltip("The actions to carry out in each interval.")]
	public UnityEvent shenanigans;

	[Header("Routine Control")]
	public bool startAIRoutine = true;
	public bool stopAIRoutine = false;
	[SerializeField]
	bool isAIRoutineRunning = false;

    bool isInitialized = false;

	int targetAmountOfShenanigans = 0;
	int shenanigansPerformed = 0;

	#region Boot and initilization
	void Start ()
    {
		CheckSingeltonInstance();
	}
    public void Initialize()
    {
        CreateObjectListing();
        isInitialized = true;
    }
    private void CheckSingeltonInstance()
	{
		instance = this;
	}
	private void CreateObjectListing()
	{
		interactables = new List<GameObject>();
		for (int i = 0; i < interactableObjectTags.Count; i++)
		{
			string tag = interactableObjectTags[i];
			interactables.AddRange(GameObject.FindGameObjectsWithTag(tag));
		}
		if (interactables.Count == 0)
		{
			Debug.LogWarning(gameObject.name + ": No interactables could be found in the scene!");
		}
	}
	#endregion

	public void IgnoreInteractableInteraction(GameObject _gameObject)
	{
		if (ignoredInteractables == null)
		{
			ignoredInteractables = new List<GameObject>();
		}
		ignoredInteractables.Add(_gameObject);
	}
	public void UnignoreInteractableInteraction(GameObject _gameObject)
	{
		if (ignoredInteractables == null)
		{
			return;
		}
		ignoredInteractables.RemoveAll(x => x == _gameObject);
	}

	// Update is called once per frame
	void Update () {

		// Start coroutine upon request, if not already running
		if (startAIRoutine)
		{
            if (!isInitialized) Initialize();
            if (!isAIRoutineRunning)
			{
				// Start AI coroutine
				print(gameObject.name + ": AI Coroutine is starting ...");
				StartCoroutine(PerformShenanigans());
			}
			else
			{
				Debug.LogWarning(gameObject.name + ": AI Coroutine is still running!");
			}
			startAIRoutine = false;
		}
	}

	#region AI Routine
	public void StartShenanigans(int amount, float minTimeDif = -1f, float maxTimeDif = -1f)
	{
		if (isAIRoutineRunning)
		{
			StopCoroutine(PerformShenanigans());
			isAIRoutineRunning = false;
		}

		shenanigansPerformed = 0;
		targetAmountOfShenanigans = amount;

		if (minTimeDif >= 0)
		{
			minTimeDifference = minTimeDif;
		}
		if (maxTimeDif >= 0 && maxTimeDif > minTimeDifference)
		{
			maxTimeDifference = maxTimeDif;
		}
		else
		{
			maxTimeDifference = minTimeDifference;
		}
		startAIRoutine = true;
	}

	private IEnumerator PerformShenanigans()
	{
		isAIRoutineRunning = true;

		while (isAIRoutineRunning)
		{
			float timeDiff = Random.Range(minTimeDifference, maxTimeDifference);
			print(gameObject.name + ": Waiting " + timeDiff + " seconds until shenanigans are performed ...");
			yield return new WaitForSeconds(timeDiff);
			if (stopAIRoutine)
			{
				print(gameObject.name + ": AI Coroutine is stopping ...");
				isAIRoutineRunning = false;
				stopAIRoutine = false;
			}
			else if (shenanigansPerformed < targetAmountOfShenanigans)
			{
				print(gameObject.name + ": Carrying out shenanigans ... :)");
				shenanigans.Invoke();
				shenanigansPerformed++;
				
				// Wait for end of frame
				yield return null;
			}
			else
			{
				print(gameObject.name + ": All shenanigans have been performed! :^ )");
				stopAIRoutine = true;
			}
		}
	}

	public void ToggleRandomViableInteractable()
	{
		// Gather a list of all interactables that could be turned on or off
		List<Interactable> viableInteractables = new List<Interactable>();
		for (int i = 0; i < interactables.Count; i++)
		{
			Interactable ia = interactables[i].GetComponent<Interactable>();
			// Interactable script is present, it's gameobject is in the scene and the bad power state is not already active
			if (ia != null && ia.isActiveAndEnabled && ia.gameObject.activeInHierarchy && ia.isPowered != ia.badPowerState && ignoredInteractables.Find(x=>x == ia.gameObject) == null)
			{
				viableInteractables.Add(ia);
			}
		}
		
		// Pick random viable interactable and turn it into it's bad power state
		if (viableInteractables.Count > 0)
		{
			Interactable selectedInteractable = viableInteractables[Random.Range(0, viableInteractables.Count)];
			selectedInteractable.ActivateBadPowerState(true);
		}
		else
		{
			Debug.LogWarning(gameObject.name + ": All viable interactables are already in their bad power state!");
		}
	}
	#endregion
}
