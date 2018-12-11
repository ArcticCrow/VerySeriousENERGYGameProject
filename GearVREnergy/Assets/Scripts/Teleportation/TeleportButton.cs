using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportButton : MonoBehaviour {

	[HideInInspector]
	public RoomInformation roomInfo;
	[HideInInspector]
	public GameObject tpPoint;

	public Button buttonComponent;
	public Image colorImage;
	public Text label;



	// Use this for initialization
	void Start () {
		buttonComponent.onClick.AddListener(Teleport);
	}

	private void Teleport()
	{
		OldGameManager.Instance.TeleportPlayer(tpPoint);
	}

	public void Setup(GameObject teleportPoint)
	{
		tpPoint = teleportPoint;
		roomInfo = tpPoint.GetComponent<RoomInformation>();
		label.text = roomInfo.roomName;
		colorImage.color = roomInfo.colorCode;
	}

	
}
