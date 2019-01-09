using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ShipAI : MonoBehaviour {

	[HideInInspector]
	public static ShipAI instance;

	[Header("General")]
	[SerializeField, Tooltip("A list of all objects that the AI should look to man  ipulate.")]
	private List<string> interactableObjectTags = new List<string>()
	{
		"Interactable"
	};
	[SerializeField, Tooltip("Listing of all found interactable objects in the scene. Will be generated on scene start.")]
	List<GameObject> interactables;

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

	// Use this for initialization
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
		//if (instance != null) Destroy(gameObject);

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

	// Update is called once per frame
	void Update () {

		// Start coroutine upon request, if not already running
		if (startAIRoutine)
		{
            if (!isInitialized) Initialize();
            if (!isAIRoutineRunning)
			{
				print(gameObject.name + ": AI Coroutine is starting ...");
				// Start AI coroutine
				StartCoroutine(PerformShenanigans());
			}
			else
			{
				Debug.LogWarning(gameObject.name + ": AI Coroutine is still running!");
			}
			startAIRoutine = false;
		}
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
			else
			{
				print(gameObject.name + ": Carrying out shenanigns ... :)");
				shenanigans.Invoke();

				// Wait for end of frame
				yield return null;
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
			if (ia != null && ia.isActiveAndEnabled && ia.gameObject.activeInHierarchy && ia.isPowered != ia.badPowerState)
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
}
