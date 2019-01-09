using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

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
    public OVRInput.Button interactionButton = OVRInput.Button.PrimaryIndexTrigger;
    public float maxInteractionRange = 20f;
    public LayerMask interactionMask;

    [Header("Game State Controls")]
    public bool startGamePlay;
	public Button startButton;
	public bool pauseGamePlay;
	public Button pauseButton;

    [Header("Ship")]
    [SerializeField]
    bool findRooms = false;
    static string ROOM_TAG = "Room";
    public RoomInformation currentRoom;
    public List<RoomInformation> rooms = new List<RoomInformation>();

    [Header("Teleportation")]
    public float fadeSpeed = 1f;
    public float fadeLength = 0.5f;
    bool teleporting = false;
    [Range(0, 1)]
    public float headRotationCompensation = 0.5f;

<<<<<<< HEAD
	[Header("Tutorial Flow")]
	public Sequence tutorialSequence;

	[Header("Gameplay Flow")]
	public List<Sequence> gameplaySequences;

	[HideInInspector]
=======
    [HideInInspector]
>>>>>>> parent of 3ca0e00... Merge branch 'Development' of https://github.com/ArcticCrow/VerySeriousENERGYGameProject into Development
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

    void Awake()
    {
        CheckSingeltonInstance();

        Initialize();
    }

    private void Start()
	{
        EnergyManager.instance.Initialize();
        ShipAI.instance.Initialize();
    }

    private void Initialize()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(new UnityEngine.Events.UnityAction(this.StartGame));
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(new UnityEngine.Events.UnityAction(this.StopGame));
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

        CreateRoomListing();
        InitializeRandom();
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
		//if (instance != null) Destroy(gameObject);

		instance = this;
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

        if (startGamePlay)
		{
			EnergyManager.instance.startEnergyRoutine = true;
			ShipAI.instance.startAIRoutine = true;

			startGamePlay = false;
		}

		if (pauseGamePlay)
		{
			EnergyManager.instance.stopEnergyRoutine = true;
			ShipAI.instance.stopAIRoutine = true;

			pauseGamePlay = false;
		}
	}

    private void GenerateRandomSeed()
    {
        seed = System.DateTime.Now.Ticks.ToString();
    }

    public void StartGame()
	{
		startGamePlay = true;
	}

	public void StopGame()
	{
		pauseGamePlay = true;
	}

    public void CreateRoomListing()
    {
        if (findRooms)
        {
            rooms = new List<RoomInformation>();

            GameObject[] foundRooms = GameObject.FindGameObjectsWithTag(ROOM_TAG);

            for (int i = 0; i < foundRooms.Length; i++)
            { 
                RoomInformation ri = foundRooms[i].GetComponent<RoomInformation>();
                if (ri != null)
                {
                    rooms.Add(ri);
                }
            }
        }
    }

    public void SwitchCurrentRoom(RoomInformation newActiveRoom)
    {
        currentRoom = newActiveRoom;
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

    IEnumerator TeleportCoroutine(Transform destTransform)
    {
        Vector3 destPosition = destTransform.position;
        Quaternion destRotation = destTransform.rotation;

        float fadeLevel = 0;

        while (fadeLevel < 1)
        {
            yield return null;
            fadeLevel += fadeSpeed * Time.deltaTime;
            fadeLevel = Mathf.Clamp01(fadeLevel);
            OVRInspector.instance.fader.SetFadeLevel(fadeLevel);
        }

        SoundControl.PlaySound(SFXClip.Teleportation);
        player.transform.position = destPosition;

        Quaternion headRotation = Quaternion.Euler(OVRManager.instance.headPoseRelativeOffsetRotation);
        Vector3 euler = headRotation.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        headRotation = Quaternion.Euler(euler);
        player.transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(headRotation), headRotationCompensation) * destRotation;
        /*print("Identiy: " + Quaternion.identity.eulerAngles + " (" + Quaternion.identity + ")"
            + "\nHead: " + headRotation.eulerAngles + " (" + headRotation + ")"
            + "\nHead Inverse: " + Quaternion.Inverse(headRotation).eulerAngles + "(" + Quaternion.Inverse(headRotation) + ")"
            + "\nDestination: " + destRotation.eulerAngles + " (" + destRotation + ")"
            + "\nSlerp Result: " + Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(headRotation), headRotationCompensation).eulerAngles + " (" + Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(headRotation), headRotationCompensation) + ")"
            + "\nMultiplied:" + player.transform.rotation.eulerAngles + " (" + player.transform.rotation + ")");
    */    
    yield return new WaitForSeconds(fadeLength);

        teleporting = false;

        while (fadeLevel > 0)
        {
            yield return null;
            fadeLevel -= fadeSpeed * Time.deltaTime;
            fadeLevel = Mathf.Clamp01(fadeLevel);
            OVRInspector.instance.fader.SetFadeLevel(fadeLevel);
        }

        yield return null;
    }

    public void RequestPointerEmphasis(bool hide = false)
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
