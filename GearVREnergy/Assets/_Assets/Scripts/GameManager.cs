using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    public OVRInput.Button interactionButton = OVRInput.Button.DpadDown;
    //public float maxInteractionRange = 20f;
    //public LayerMask interactionMask;

	[Header("Game State Controls")]
	public State state = State.Boot; // not used yet
	public bool playGame;
	public bool pause;

	public bool enableTutorial = true;
	public bool enableCore = true;
	public bool enableEnding = true;

	[Header("Menu Buttons")]
	public Button launchTutButton;
	public Button playButton;
	public Button pauseButton;

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

	float fadeLevel = 0;

	// Gameplay and core game loop
	[Header("Gameplay Flow")]
	public Sequence tutorialSequence;
	public List<Sequence> gameplaySequences;
	public Sequence endSequence;

	public int startAmountOfShenanigans = 0;
	public int shenanigansStepIncrease = 1;
	int amountOfShenanigans;

	Sequence activeSequence;

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
    }

    private void Initialize()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }
		if (launchTutButton != null)
		{
			launchTutButton.onClick.AddListener(PlayTutorial);
		}
		if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(Pause);
        }
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

		EnergyManager.Instance.Initialize();
		ShipAI.Instance.Initialize();
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

	private IEnumerator GameplayLoop()
	{
		// Tutorial Sequence
		if (enableTutorial)
		{
			if (tutorialSequence == null)
			{
				throw new Exception("No tutorial sequence has been set up!");
			}
			activeSequence = tutorialSequence;
			activeSequence.LaunchSequence();

			while (!activeSequence.isSequenceFinished) yield return null;
		}

		// Core Game Loop - Gameplay sequences in random order with increasing challenge
		if (enableCore)
		{
			if (gameplaySequences == null)
			{
				throw new Exception("No gameplay sequences have been set up!");
			}
			if (gameplaySequences.Count > 0)
			{
				amountOfShenanigans = startAmountOfShenanigans;
				List<Sequence> availableSequences = new List<Sequence>(gameplaySequences);
				while (availableSequences.Count > 0)
				{
					// Pick and remove random sequence from available
					int sequenceIndex = Random.Range(0, availableSequences.Count);
					activeSequence = availableSequences[sequenceIndex];
					availableSequences.Remove(activeSequence);

					// Setup additional ai behaviour
					ShipAI.Instance.StartShenanigans(amountOfShenanigans);
					amountOfShenanigans += shenanigansStepIncrease;

					// Start sequence
					activeSequence.LaunchSequence();
					while (!activeSequence.isSequenceFinished) yield return null;

					// Stop AI routine after finishing sequence
					ShipAI.Instance.StopAllCoroutines();
				}
			}
		}

		// Outro Sequence
		if (enableEnding)
		{
			if (endSequence == null)
			{
				throw new Exception("No end sequence has been set up!");
			}
			activeSequence = endSequence;
			activeSequence.LaunchSequence();

			while (!activeSequence.isSequenceFinished) yield return null;
		}

		yield return null;
	}

	public void ActivateInsaneAIMode()
	{
		ShipAI.Instance.StopAllCoroutines();
		ShipAI.Instance.StartShenanigans(99, 0.5f, 2f);
	}

	private void PlayGame()
	{
		// Start Game - Load if necessary
		StartCoroutine(GameplayLoop());
	}

	private void Pause()
	{
		// Pause
		EnergyManager.Instance.stopEnergyRoutine = true;
		ShipAI.Instance.stopAIRoutine = true;
		StopCoroutine(GameplayLoop());
	}

	private void ResetGame()
	{
		
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
		if (teleporting) return;
		teleporting = true;

		SwitchCurrentRoom(room);

		Transform destTransform = currentRoom.playerTeleportTransform;
		StartCoroutine(TeleportCoroutine(destTransform));
	}

    public void TeleportToOtherRoom(RoomInformation roomA, RoomInformation roomB)
    {
        if (teleporting) return;
        teleporting = true;

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
        StartCoroutine(TeleportCoroutine(destTransform));
    }
    IEnumerator TeleportCoroutine(Transform destTransform, bool ignoreFade = false)
    {
        Vector3 destPosition = destTransform.position;
        Quaternion destRotation = destTransform.rotation;

		if (!ignoreFade)
		yield return FadeOut();

        SoundController.PlaySound(SFXClip.Teleportation);
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
