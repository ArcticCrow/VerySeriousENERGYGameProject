using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public static GameManager instance;

    [Header("General")]
    public Transform pointerTransform;
	public GameObject player;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    public OVRInput.Button interactionButton = OVRInput.Button.One;
    public float maxInteractionRange = 20f;
    public LayerMask interactionMask;

    [Header("Game Controls")]
    public bool startGamePlay;
	public Button startButton;
	public bool pauseGamePlay;
	public Button pauseButton;

	private void Start()
	{
		CheckSingeltonInstance();
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
	}

	private void CheckSingeltonInstance()
	{
		//if (instance != null) Destroy(gameObject);

		instance = this;
	}

	// Update is called once per frame
	void Update () {
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

	public void StartGame()
	{
		startGamePlay = true;

	}

	public void StopGame()
	{
		pauseGamePlay = true;

	}
}
