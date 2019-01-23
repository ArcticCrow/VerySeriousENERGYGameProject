using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour 
{
	public GameObject activePlanet;
	public void Update()
	{
		bool foundThreshhold = false;
		for (int i = GameManager.instance.journeyThreshholds.Count - 1; i >= 0; i--)
		{
			if (!foundThreshhold && (i == 0 
				|| (EnergyManager.Instance.energyUsedThisRun >= GameManager.instance.journeyThreshholds[i-1].energyAmount
				&& (EnergyManager.Instance.energyUsedThisRun <= GameManager.instance.journeyThreshholds[i].energyAmount
				|| i == GameManager.instance.journeyThreshholds.Count - 1))))
			{
				activePlanet = GameManager.instance.journeyThreshholds[i].planet;
				activePlanet.SetActive(true);
				foundThreshhold = true;
			}
			else
			{
				GameManager.instance.journeyThreshholds[i].planet.SetActive(false);
			}
		}
	}
}