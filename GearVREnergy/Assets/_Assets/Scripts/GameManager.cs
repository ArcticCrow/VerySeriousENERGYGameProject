using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

	public enum State
	{
		Boot,
		Menu,
		Setup,
		Tutorial,
		MainGame,
		Pause,
		Finish,
		EndScreen,
	}

	[HideInInspector]
	public static GameManager instance;

    [Header("General")]
    [SerializeField]
    Transform leftHandAnchor = null;
    [SerializeField]
    Transform rightHandAnchor = null;
    [SerializeField]
    Transform centerEyeAnchor = null;

    public GameObject player;

    [Tooltip("Create a random seed base on system time.")]
    public bool generateRandomSeed = false;
    [Tooltip("The seed for the random number generator. (Mainly used for the ship AI)")]
    public string seed = "58008";

	[Header("Game Settings")]
	public int runsToReachPlanet = 3;
	public int numberOfCurrentRun = 1;

	[Serializable]
	public struct JourneyThreshhold
	{
		public float energyAmount;
		public Color colorCode;
	}

	public float maxAmountOfEnergyPerRun;
	public List<JourneyThreshhold> journeyThreshholds = new List<JourneyThreshhold>();

	public UnityEvent runStartEvents;
	public UnityEvent runCompletionEvents;

	[Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    public OVRInput.Button interactionButton = OVRInput.Button.One;

	[Header("Game State Controls")]
	public State state = State.Boot;
	State resumeState;
	bool isPlaying = false;

	public bool playGame;
	public bool pause;

	public bool enableTutorial = true;
	public bool enableCore = true;
	public bool enableEnding = true;

	// Ships rooms with information
    [Header("Layout")]
    [SerializeField]
    bool findRooms = false;
    static string ROOM_TAG = "Room";
    public RoomInformation currentRoom;
    public List<RoomInformation> rooms;

	// Teleporation tweaking
    [Header("Teleportation")]
    public float fadeSpeed = 1f;
    public float fadeLength = 0.5f;
    bool teleporting = false;
    [Range(0, 1)]
    public float headRotationCompensation = 0.5f;
    float teleportTimestamp = 0f;

	float fadeLevel = 0;

	// Gameplay and core game loop
	[Header("Gameplay Flow")]
	public Sequence tutorialSequence;
	public List<Sequence> gameplaySequences;
	public Sequence endSequence;

	public int startAmountOfShenanigans = 0;
	public int shenanigansStepIncrease = 1;
	int amountOfShenanigans;

	[HideInInspector]
	public Sequence activeSequence;

	List<Sequence> availableSequences;

	// Controllers
	[HideInInspector]
	public InteractionController interactionControl;
	[HideInInspector]
	public DoorController doorControl;
	[HideInInspector]
	public ThrowController throwControl;

	// VR Pointer
	[HideInInspector]
    public bool pointerIsRemote;
    public Transform Pointer
    {
        get
        {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers();
            pointerIsRemote = true;
            if ((controller & OVRInput.Controller.LTrackedRemote) != OVRInput.Controller.None)
            {
                return leftHandAnchor;
            }
            else if ((controller & OVRInput.Controller.RTrackedRemote) != OVRInput.Controller.None)
            {
                return rightHandAnchor;
            }
            // If no controllers are connected, we use ray from the view camera. 
            // This looks super ackward! Should probably fall back to a simple reticle!
            pointerIsRemote = false;
            return centerEyeAnchor;
        }
    }

	#region Boot
	void Awake()
    {
        CheckSingeltonInstance();
        Initialize();
		DontDestroyOnLoad(gameObject);
    }

    private void Initialize()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        // Move player to spawn point (current room) if set
        if (currentRoom != null)
        {
            player.transform.position = currentRoom.playerTeleportTransform.position;
            player.transform.rotation = currentRoom.playerTeleportTransform.rotation;
        }
        if (leftHandAnchor == null)
        {
            Debug.LogWarning("Assign LeftHandAnchor in the inspector!");
            GameObject left = GameObject.Find("LeftHandAnchor");
            if (left != null)
            {
                leftHandAnchor = left.transform;
            }
        }
        if (rightHandAnchor == null)
        {
            Debug.LogWarning("Assign RightHandAnchor in the inspector!");
            GameObject right = GameObject.Find("RightHandAnchor");
            if (right != null)
            {
                rightHandAnchor = right.transform;
            }
        }
        if (centerEyeAnchor == null)
        {
            Debug.LogWarning("Assign CenterEyeAnchor in the inspector!");
            GameObject center = GameObject.Find("CenterEyeAnchor");
            if (center != null)
            {
                centerEyeAnchor = center.transform;
            }
        }

		if (interactionControl == null)
		{
			interactionControl = GetComponent<InteractionController>();
			if (interactionControl == null)
			{
				interactionControl = gameObject.AddComponent<InteractionController>();
			}
		}
		if (doorControl == null)
		{
			doorControl = GetComponent<DoorController>();
			if (doorControl == null)
			{
				doorControl = gameObject.AddComponent<DoorController>();
			}
		}
		if (throwControl == null)
		{
			throwControl = GetComponent<ThrowController>();
			if (throwControl == null)
			{
				throwControl = gameObject.AddComponent<ThrowController>();
			}
		}

		if (playGame && enableTutorial)
		{
			interactionControl.DisableAllInteractions();
			doorControl.DisableAllDoorways();
		}

		CreateRoomListing();
        InitializeRandom();

		if (journeyThreshholds.Count > 0)
		{
			journeyThreshholds.Sort(delegate (JourneyThreshhold a, JourneyThreshhold b) {
				return (a.energyAmount).CompareTo(b.energyAmount);
			});
		}

		EnergyManager.Instance.Initialize();
		ShipAI.Instance.Initialize();

        if (pointerIsRemote) OVRInput.RecenterController();
    }

	
	private void InitializeRandom()
    {
        if (generateRandomSeed)
        {
            GenerateRandomSeed();
            generateRandomSeed = false;
        }
        Random.InitState(seed.GetHashCode());
    }

    private void CheckSingeltonInstance()
	{
		if (instance != null) DestroyImmediate(gameObject);
		instance = this;
	}

	public void CreateRoomListing()
	{
		if (findRooms)
		{
			rooms = new List<RoomInformation>();

			GameObject[] foundRooms = GameObject.FindGameObjectsWithTag(ROOM_TAG);

			for (int i = 0; i < foundRooms.Length; i++)
			{
				RoomInformation room = foundRooms[i].GetComponent<RoomInformation>();
				if (room != null)
				{
					rooms.Add(room);
				}
			}
		}
	}

	private void GenerateRandomSeed()
	{
		seed = System.DateTime.Now.Ticks.ToString();
	}
	#endregion

	// Update is called once per frame
	void Update () {
        if (state == State.Boot)
		{
			OVRInspector.instance.fader.SetFadeLevel(1);
			state = State.Menu;
		}
		
		// If a new seed is requested during a playsession, regenerate a new seed and reinitialize random
        if (generateRandomSeed)
        {
            GenerateRandomSeed();
            InitializeRandom();
            generateRandomSeed = false;
        }

        if (playGame)
		{
			PlayGame();
			playGame = false;
		}

		if (pause)
		{
			Pause();
			pause = false;
		}
	}

	private void StartMenu()
	{
		// Fade in main menu
		state = State.Menu;
	}

	private void ReturnToMenu()
	{
		// Save sequence & progress if necessary
		Pause();
		StartMenu();
	}

	private void PlayTutorial()
	{
		enableTutorial = true;
		PlayGame();
	}

	private void PlayGame()
	{
		// Start Game - Load if necessary
		StartCoroutine(GameplayLoop());
	}

	public void Pause()
	{
		if (state != State.Pause)
		{
			FadeScreenOut();
			resumeState = state;
			state = State.Pause;

			interactionControl.DisableAllInteractions();
			doorControl.DisableAllDoorways();
			throwControl.disableThrowing = true;

			// Pause
			EnergyManager.Instance.pauseEnergyRoutine = true;
			ShipAI.Instance.pauseAIRoutine = true;
		}
	}

	public void Resume()
	{
		if (state == State.Pause)
		{
			FadeScreenIn();
			state = resumeState;

			interactionControl.EnableAllInteractions();
			doorControl.EnableAllDoorways();
			throwControl.disableThrowing = false;

			// Pause
			EnergyManager.Instance.pauseEnergyRoutine = false;
			ShipAI.Instance.pauseAIRoutine = false;
		}
	}

	private void ResetGame()
	{
		EnergyManager.Instance.SetMultiplier(1f);
        if (numberOfCurrentRun >= runsToReachPlanet || numberOfCurrentRun <= 0)
        {
            Debug.Log("All runs completed. Show final result to player(s)!");
            numberOfCurrentRun = 1;
        }
        else
        {
            numberOfCurrentRun++;
        }
    }

	public void ShowEndScreen()
	{

	}

	private IEnumerator SetupTutorial()
	{
		if (tutorialSequence == null)
		{
			throw new Exception("No tutorial sequence has been set up!");
		}

		state = State.Tutorial;

		BGMController.instance.SetPitch(0);
		BGMController.instance.PlayTutorialBGM();
		BGMController.instance.PitchIn(1f);

		ShipAI.Instance.ResetIgnoredInteractables();
		EnergyManager.Instance.startEnergyRoutine = true;

        availableSequences = new List<Sequence>
        {
            tutorialSequence
        };
        yield return null;
	}

	private IEnumerator SetupCore()
	{
		if (gameplaySequences == null)
		{
			throw new Exception("No gameplay sequences have been set up!");
		}

		if (gameplaySequences.Count > 0)
		{
			state = State.MainGame;

			BGMController.instance.SetPitch(0);
			BGMController.instance.PlayCoreBGM();
			BGMController.instance.PitchIn(1f);

			ShutdownAllInteractables(false);

			EnergyManager.Instance.startEnergyRoutine = true;

			amountOfShenanigans = startAmountOfShenanigans;

			availableSequences = new List<Sequence>(gameplaySequences);
			yield return null;
		}
	}

	private IEnumerator SetupEnding()
	{
		if (endSequence == null)
		{
			throw new Exception("No end sequence has been set up!");
		}

		state = State.Finish;

		// Let AI Interact with everything
		ShipAI.Instance.ResetIgnoredInteractables();
		BGMController.instance.SetPitch(0);
		BGMController.instance.PlayInsaneBGM();
		BGMController.instance.PitchIn(5f);

        availableSequences = new List<Sequence>
        {
            endSequence
        };
        yield return null;
	}

	public IEnumerator FinishTutorial()
	{
		EnergyManager.Instance.stopEnergyRoutine = true;
		EnergyManager.Instance.ResetEnergyUsedThisRun();

		BGMController.instance.PitchOut(1f);
		yield return new WaitForSeconds(1f);
	}
	public IEnumerator FinishCore()
	{
		// Stop AI routine after finishing sequence
		ShipAI.Instance.StopAllCoroutines();

		BGMController.instance.PitchOut(1f);
		yield return new WaitForSeconds(1f);
	}
	public IEnumerator FinishEnding()
	{
		isPlaying = false;
		yield return null;
	}

	private IEnumerator GameplayLoop()
	{
		isPlaying = true;
		ResetGame();
		if (runStartEvents != null) runStartEvents.Invoke();

		bool finishedCurrentSequence = false;
		bool setupCurrentSequence = false;

		bool playTut = enableTutorial,
			playCore = enableCore,
			playEnd = enableEnding;

		while (isPlaying)
		{
			if (state == State.Pause)
			{
				yield return new WaitForEndOfFrame();
				continue;
			}

			if (!setupCurrentSequence)
			{
				if (playTut)
				{
					yield return SetupTutorial();
					playTut = false;
				}
				else if (playCore)
				{
					yield return SetupCore();
					playCore = false;
				}
				else if (playEnd)
				{
					yield return SetupEnding();
					playEnd = false;
				}
				setupCurrentSequence = true;
				finishedCurrentSequence = false;
			}

			if (availableSequences.Count > 0 && (activeSequence == null || activeSequence.isSequenceFinished))
			{
				// Pick and remove random sequence from available
				int sequenceIndex = Random.Range(0, availableSequences.Count);
				activeSequence = availableSequences[sequenceIndex];

				availableSequences.Remove(activeSequence);

				switch (state)
				{
					case State.Tutorial:
						break;

					case State.MainGame:
						// Stop AI routine after finishing sequence
						ShipAI.Instance.StopAllCoroutines();

						// Let AI Interact with everything
						ShipAI.Instance.ResetIgnoredInteractables();

						// Setup additional ai behaviour
						ShipAI.Instance.StartShenanigans(amountOfShenanigans);
						amountOfShenanigans += shenanigansStepIncrease;
						break;

					case State.Finish:
						break;
					default:
						break;
				}

				Debug.Log("Playing sequence " + activeSequence.name);
				activeSequence.LaunchSequence();
			}

			finishedCurrentSequence = activeSequence.isSequenceFinished;
			yield return new WaitForEndOfFrame();

			if (finishedCurrentSequence)
			{
				if (availableSequences == null || availableSequences.Count == 0)
				{
					switch (state)
					{
						case State.Tutorial:
							yield return FinishTutorial();
							break;
						case State.MainGame:
							yield return FinishCore();
							break;
						case State.Finish:
							yield return FinishEnding();
							break;
						default:
							break;
					}
					setupCurrentSequence = false;
				}
			}
		}

        ResetGame();
		if (runCompletionEvents != null) runCompletionEvents.Invoke();

		yield return null;
	}

	public void ActivateInsaneAIMode()
	{
		ShipAI.Instance.StopAllCoroutines();
		ShipAI.Instance.StartShenanigans(99, 1f, 1.5f);
		EnergyManager.Instance.SetMultiplier(0.3f);
	}

	public void ShutdownAllInteractables(bool disableInteraction)
	{
		if (disableInteraction)
		{
			interactionControl.DisableAllInteractions();
		}
		for (int i = 0; i < rooms.Count; i++)
		{
			RoomInformation room = rooms[i];
			for (int j = 0; j < room.interactables.Count; j++)
			{
				Interactable interactable = room.interactables[j];
				if (!interactable.disableInteraction)
					interactable.SetPowerState(false);
			}
		}
	}

	public void ActivateAllInteractables(bool enableInteraction)
	{
		if (enableInteraction)
		{
			interactionControl.DisableAllInteractions();
		}
		for (int i = 0; i < rooms.Count; i++)
		{
			RoomInformation room = rooms[i];
			for (int j = 0; j < room.interactables.Count; j++)
			{
				Interactable interactable = room.interactables[j];
				if (!interactable.disableInteraction)
					interactable.SetPowerState(true);
			}
		}
	}

	#region Movement/Teleportation 


	public void SwitchCurrentRoom(RoomInformation newActiveRoom)
    {
        currentRoom = newActiveRoom;
    }
	public void TeleportToRoom(RoomInformation room)
	{
		if (IsTeleporting()) return;

        SwitchCurrentRoom(room);

		Transform destTransform = currentRoom.playerTeleportTransform;

        teleporting = true;
        teleportTimestamp = Time.time;
        StartCoroutine(TeleportCoroutine(destTransform));
	}

    public void TeleportToOtherRoom(RoomInformation roomA, RoomInformation roomB)
    {
        if (IsTeleporting()) return;
        

        if (currentRoom == roomA)
        {
            SwitchCurrentRoom(roomB);
        }
        else if (currentRoom == roomB)
        {
            SwitchCurrentRoom(roomA);
        }
        else
        {
            throw new Exception("Player is trying to teleport out of a room, he is not in!");
        }

        Transform destTransform = currentRoom.playerTeleportTransform;

        teleporting = true;
        teleportTimestamp = Time.time;
        StartCoroutine(TeleportCoroutine(destTransform));
    }

    public bool IsTeleporting()
    {
        return teleporting && Time.time - teleportTimestamp > fadeLength * 2;
    }

    IEnumerator TeleportCoroutine(Transform destTransform, bool ignoreFade = false)
    {
        Vector3 destPosition = destTransform.position;
        Quaternion destRotation = destTransform.rotation;

		if (!ignoreFade)
		yield return FadeOut();

        SFXController.PlaySound(SFXController.instance.soundEffects.teleportation);
        player.transform.position = destPosition;

        Quaternion headRotation = Quaternion.Euler(OVRManager.instance.headPoseRelativeOffsetRotation);
        Vector3 euler = headRotation.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        headRotation = Quaternion.Euler(euler);
        player.transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(headRotation), headRotationCompensation) * destRotation;
		    
		yield return new WaitForSeconds(fadeLength);

        teleporting = false;

		if (!ignoreFade)
		yield return FadeIn();

        yield return null;
    }

	public void FadeScreenIn()
	{
		StartCoroutine(FadeIn());
	}
	public void FadeScreenOut()
	{
		StartCoroutine(FadeOut());
	}

	IEnumerator FadeOut()
	{
		StopCoroutine(FadeIn());
		while (fadeLevel < 1)
		{
			yield return null;
			fadeLevel += fadeSpeed * Time.deltaTime;
			fadeLevel = Mathf.Clamp01(fadeLevel);
			OVRInspector.instance.fader.SetFadeLevel(fadeLevel);
		}
	}

	IEnumerator FadeIn()
	{
		StopCoroutine(FadeOut());
		while (fadeLevel > 0)
		{
			yield return null;
			fadeLevel -= fadeSpeed * Time.deltaTime;
			fadeLevel = Mathf.Clamp01(fadeLevel);
			OVRInspector.instance.fader.SetFadeLevel(fadeLevel);
		}
	}
	#endregion

	public void RequestPointerEmphasis(bool badEmphasis = false, bool hide = false)
    {
        if (hide)
        {
            if (Pointer != centerEyeAnchor)
            {
                // Assuming that the pointer is a remote
                VRRaycaster.instance.RequestDarken();
            }
            else
            {
                // Assuming that the user isn't using any remote
                OVRGazePointer.instance.RequestHide();
            }
        }
        else if (badEmphasis)
		{
			if (Pointer != centerEyeAnchor)
			{
				// Assuming that the pointer is a remote
				VRRaycaster.instance.RequestBadIllumination();
			}
			else
			{
				// Assuming that the user isn't using any remote
				OVRGazePointer.instance.RequestHide();
			}
		}
		else
        {
            if (Pointer != centerEyeAnchor)
            {
                // Assuming that the pointer is a remote
                VRRaycaster.instance.RequestIllumination();
            }
            else
            {
                // Assuming that the user isn't using any remote
                OVRGazePointer.instance.RequestShow();
            }
        }
    }
}
