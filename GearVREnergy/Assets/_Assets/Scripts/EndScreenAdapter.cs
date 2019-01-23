using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class EndScreenAdapter : EnergyDisplay
{
	[Header("Menu/Stats Display")]
	public TextMeshProUGUI statDisplay;
	public Button exitButton;

	public override void Update()
	{
		base.Update();

		statDisplay.text = GameManager.instance.numberOfCurrentRun + ". of " + GameManager.instance.runsToReachPlanet
			+ " runs to reach the next planet.\nIt took you " + (GameManager.instance.endTimestamp - GameManager.instance.startTimestamp) 
			+ " seconds to complete your run!.";
	}
}