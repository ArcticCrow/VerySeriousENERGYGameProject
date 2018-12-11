﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ShipAI : MonoBehaviour {

	[Header("Initial Properties")]
	[SerializeField, Tooltip("A list of all objects that the AI should look to manipulate.")]
	private List<string> interactableObjectTags = new List<string>()
	{
		"Interactable"
	};
	[SerializeField, Tooltip("Listing of all found interactable objects in scene. Will be generated on scene start.")]
	List<GameObject> interactables;
	[Tooltip("Create a random seed base on system time.")]
	public bool generateRandomSeed = false;
	[Tooltip("The seed for the random number generator.")]
	public string seed = "58008";

	[Header("Interval Control")]
	[Tooltip("The minimum time difference, before the AI makes its next move.")]
	public float minTimeDifference = 5f;
	[Tooltip("The maximum time difference, before the AI makes its next move.")]
	public float maxTimeDifference = 10f;
	[Tooltip("The actions to carry out in each interval.")]
	public UnityEvent shenanigans;

	[Header("Routine Control")]
	[SerializeField]
	bool startAIRoutine = true;
	[SerializeField]
	bool stopAIRoutine = false;
	[SerializeField]
	bool isAIRunning = false;


	// Use this for initialization
	void Start () {
		if (generateRandomSeed)
		{
			GenerateRandomSeed();
			generateRandomSeed = false;
		}
		InitializeRandom();
		CreateInteractablesListing();
	}

	private void CreateInteractablesListing()
	{
		interactables = new List<GameObject>();
		for (int i = 0; i < interactableObjectTags.Count; i++)
		{
			string tag = interactableObjectTags[i];
			interactables.AddRange(GameObject.FindGameObjectsWithTag(tag));
		}
		if (interactables.Count == 0)
		{
			throw new Exception("No interactables could be found in the scene!");
		}
	}

	private void InitializeRandom()
	{
		Random.InitState(seed.GetHashCode());
	}

	private void GenerateRandomSeed()
	{
		seed = System.DateTime.Now.Ticks.ToString();
	}

	// Update is called once per frame
	void Update () {
		// If a new seed is requested during a playsession, regenerate a new seed and reinitialize random
		if (generateRandomSeed)
		{
			GenerateRandomSeed();
			InitializeRandom();
			generateRandomSeed = false;
		}

		// Start coroutine upon request, if not already running
		if (startAIRoutine)
		{
			if (!isAIRunning)
			{
				print("AI Coroutine is starting ...");
				// Start AI coroutine
				StartCoroutine(PerformShenanigans());
			}
			else
			{
				Debug.LogWarning("AI Coroutine is still running!");
			}
			startAIRoutine = false;
		}
	}

	private IEnumerator PerformShenanigans()
	{
		isAIRunning = true;

		while (isAIRunning)
		{
			float timeDiff = Random.Range(minTimeDifference, maxTimeDifference);
			print("Waiting " + timeDiff + " seconds until shenanigans are performed ...");
			yield return new WaitForSeconds(timeDiff);

			print("Carrying out shenanigns ... :)");

			if (stopAIRoutine)
			{
				print("AI Coroutine is stopping ...");
				isAIRunning = false;
				stopAIRoutine = false;
			}
			else
			{
				shenanigans.Invoke();

				// Wait for end of frame
				yield return null;
			}
		}
	}

	// Just for testing ai toggles
	public void TogglePlaceholderDevice()
	{
		bool objToggleSuccess = false;
		while (!objToggleSuccess)
		{
			// Choose a random interactable
			GameObject obj = interactables[Random.Range(0, interactables.Count)];
			// Determine if object is already in "bad state" (replaced by a random value check for testing)
			if (Random.Range(0, 2) == 1)
			{
				objToggleSuccess = true;
				MeshRenderer mr = obj.GetComponent<MeshRenderer>();
				Color currentColor = mr.material.color;
				Color onColor = Color.white, offColor = Color.black;

				if (currentColor == onColor)
				{
					mr.material.color = offColor;
					print("Turned " + obj.name + "->" + obj.transform.parent.parent.name + "off!");
				}
				else
				{
					mr.material.color = onColor;
					print("Turned " + obj.name + "->" + obj.transform.parent.parent.name + "on!");
				}
			}
		}
	}
}
