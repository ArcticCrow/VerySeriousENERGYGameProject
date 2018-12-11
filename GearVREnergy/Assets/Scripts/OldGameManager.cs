using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldGameManager : MonoBehaviour {

	// Singelton object instance for other scripts to access
	[HideInInspector]
	public static OldGameManager Instance;


	enum PlayMode
	{
		Computer,
		VR
	}
	enum GameState
	{
		Boot,
		Load,
		Run,
		Pause,
		End
	}

	private const string TP_POINT_TAG = "Teleport Point";
	private const string PLAYER_TAG = "Player";

	
    #region Inspector visible variables
	// The players transform made available for other scripts
	public GameObject player;

	[Header("States")]
	// What the player is using to play the game
	[SerializeField]
	PlayMode playMode = PlayMode.Computer;

	// The current state of the game
	[SerializeField]
	GameState gameState = GameState.Boot;

	[Header("Teleportation")]
    public GameObject spawnPoint;
    // List of all game objects (points) the player may teleport too
    [SerializeField]
	List<GameObject> teleportPoints;

	[Header("User Interface (Computer)")]
	// The ui the player sees, when using a computer
	[SerializeField]
	Transform computerCanvas;

	// The ui the player sees, when using a computer
	[SerializeField]
	Transform teleportButtonPanel;

	// The currently active room text
	[SerializeField]
	Text currentRoomDisplay;

	public SimpleObjectPool buttonObjectPool;
    #endregion

    // Currently active Teleport Point
    GameObject activeTeleportPoint;

    // The active scene camera
    Camera mainCamera;
    OVRScreenFade sF;

	bool isTeleporting = false;


    // Use this for initialization
    void Start () {
		CheckSingeltonInstance();

		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag(PLAYER_TAG);

			if (player == null)
				throw new Exception("No player has been set in the Game Manager or could be found!");
		}

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            sF = mainCamera.transform.GetComponent<OVRScreenFade>();
        }

        InitializeTeleportation();

		if (spawnPoint != null && spawnPoint.GetComponent<RoomInformation>() != null)
		{
			TeleportPlayer(spawnPoint);
		}

		gameState = GameState.Load;
	}

	private void CheckSingeltonInstance()
	{
		if (Instance != null)
			Destroy(gameObject);

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void InitializeTeleportation()
	{
		// Get all tp points from the scene by tag
		teleportPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag(TP_POINT_TAG));
	
		// Add an on screen hud if the player is using a computer
		if (playMode == PlayMode.Computer)
		{
			computerCanvas.gameObject.SetActive(true);
		}
		else if (playMode == PlayMode.VR)
		{
			if (computerCanvas != null)
			computerCanvas.gameObject.SetActive(false);
		}

		RefreshDisplays();
	}

	public void RefreshDisplays ()
	{
		if (playMode == PlayMode.Computer)
		{
			RefreshComputerUI();
		}
		else if (playMode == PlayMode.VR)
		{
			// TODO Update onboard ui if necessarry
		}
	}

	private void RefreshComputerUI()
	{
		if (computerCanvas == null)
		{
			computerCanvas = GameObject.FindGameObjectWithTag("Computer UI").transform;
		}
		RemoveUIElements();
		AddTeleportButtons();
	}

	private void RemoveUIElements()
	{
		while (teleportButtonPanel.childCount > 0)
		{
			GameObject toRemove = teleportButtonPanel.GetChild(0).gameObject;
			buttonObjectPool.ReturnObject(toRemove);
		}
	}

	private void AddTeleportButtons()
	{
		for (int i = 0; i < teleportPoints.Count; i++)
		{
			GameObject tpp = teleportPoints[i];
			GameObject newButton = buttonObjectPool.GetObject();
			newButton.transform.SetParent(teleportButtonPanel);

			TeleportButton tpButton = newButton.GetComponent<TeleportButton>();
			tpButton.Setup(tpp);
		}
		if (activeTeleportPoint != null)
		{
			RoomInformation activeInfo = activeTeleportPoint.GetComponent<RoomInformation>();
			currentRoomDisplay.text = activeInfo.roomName;
			currentRoomDisplay.color = activeInfo.colorCode;
		}
	}

	public void TeleportPlayer(GameObject teleportPoint)
	{
		// Add last active teleport point back to the list of available points
		if (activeTeleportPoint != null)
		{
			teleportPoints.Add(activeTeleportPoint);
		}

		// Set and remove the selected point
		activeTeleportPoint = teleportPoint;
		teleportPoints.Remove(teleportPoint);


        // Move the player transform to the desired location
        Vector3 newPosition = teleportPoint.transform.position;
        if (gameState == GameState.Boot)
        {
            sF.SetFadeLevel(1);
            player.transform.position = newPosition;
            sF.FadeIn();
        }
        else
        {
            StartCoroutine(PlayerTransition(newPosition));
        }

        RefreshDisplays();
    }

    public IEnumerator PlayerTransition(Vector3 newPosition)
    {
        // Fade out screen only if screen is not faded
        sF.FadeOut();
        yield return new WaitForSeconds(sF.fadeTime + 0.1f);

        // Move the player transform to the desired location
        player.transform.position = newPosition;

		// Fade in screen
		isTeleporting = false;
        sF.FadeIn();
        yield return new WaitForSeconds(sF.fadeTime);
    }
}
