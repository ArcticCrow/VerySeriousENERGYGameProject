using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;


public class EnergyDisplay : MonoBehaviour {

	[Header("Journey Display")]
	public float journeyWidth;

	public Transform journeyContainer;
	public GameObject journeyRunEnergyBarPrefab;

	public Transform threshholdContainer;
	public GameObject threshholdPrefab;

	[Header("Run Display")]
	public Image playerAvatar;

	public float runBarWidth;
	public Transform currentRunEnergyBar;

	[Serializable]
	public class JourneyEnergyBar
	{
		public Color color;
		public GameObject go;
		public Image bar;

		public JourneyEnergyBar(Color col, GameObject go)
		{
			this.go = go;
			this.bar = go.GetComponent<Image>();
			ApplyColor(col);
		}

		public void ApplyColor(Color col)
		{
			this.color = col;
			this.bar.color = col;
		}
	}

	[Header("Read-only (Do not touch)")]
	[SerializeField]
	public List<JourneyEnergyBar> journeyEnergyBars = new List<JourneyEnergyBar>();
	[SerializeField]
	public Color currentColor;
	[SerializeField]
	public float maxEnergyAmount;
	[SerializeField]
	public List<GameObject> threshholds = new List<GameObject>();

	public virtual void Update()
	{
		maxEnergyAmount = GameManager.instance.journeyThreshholds[GameManager.instance.journeyThreshholds.Count - 1].energyAmount;

		if (journeyContainer == null 
			|| currentRunEnergyBar == null 
			|| journeyRunEnergyBarPrefab == null
			|| EnergyManager.Instance.energyConsumedDuringJourney == null)
		{
			return;
		}

		if (threshholds == null || threshholds.Count == 0)
		{
			threshholds = new List<GameObject>();
			for (int i = 0; i < GameManager.instance.journeyThreshholds.Count; i++)
			{
				SetupThreshhold(i);
			}
		}
		else
		{
			for (int i = 0; i < threshholds.Count; i++)
			{
				UpdateThreshhold(threshholds[i], i);
			}
		}

		if (journeyEnergyBars != null)
		{
			for (int i = 0; i < journeyEnergyBars.Count; i++)
			{
				UpdateJourneyBar(journeyEnergyBars[i], i);
			}
		}

		Vector3 scale = currentRunEnergyBar.localScale;
		scale.x = CalculatePercentage(EnergyManager.Instance.energyUsedThisRun, GameManager.instance.maxAmountOfEnergyPerRun);
		currentRunEnergyBar.localScale = scale;
	}

	public void AddNewPlayer()
	{
		if (journeyEnergyBars != null) journeyEnergyBars = new List<JourneyEnergyBar>();
		for (int i = 0; i < EnergyManager.Instance.energyConsumedDuringJourney.Count; i++)
		{
			if (i >= journeyEnergyBars.Count)
			SetupJourneyBar(i);
		}
	}

	private void UpdateJourneyBar(JourneyEnergyBar journeyEnergyBar, int i)
	{
		float energy = EnergyManager.Instance.energyConsumedDuringJourney[i];
		Vector2 size = journeyEnergyBar.go.GetComponent<RectTransform>().sizeDelta;
		size.x = CalculatePositionInLine(journeyWidth, energy, maxEnergyAmount);
		journeyEnergyBar.go.GetComponent<RectTransform>().sizeDelta = size;
	}

	private void UpdateThreshhold(GameObject threshhold, int i)
	{
		float energy = GameManager.instance.journeyThreshholds[i].energyAmount;
		Color col = GameManager.instance.journeyThreshholds[i].colorCode;

		threshhold.GetComponent<RectTransform>().localPosition = new Vector2(CalculatePositionInLine(journeyWidth, energy, maxEnergyAmount), 0);
		threshhold.GetComponent<Image>().color = col;
	}

	private float CalculatePositionInLine(float width, float currentValue, float maxValue)
	{
		/*print("width: " + width + "; currentValue: " + currentValue + "; maxValue: " + maxValue
			+ "\nPercentage: " + CalculatePercentage(currentValue, maxValue) * 100
			+ "\nResult: " + Mathf.Lerp(0, width, CalculatePercentage(currentValue, maxValue)));*/
		return Mathf.Lerp(0, width, CalculatePercentage(currentValue, maxValue));
	}

	private float CalculatePercentage(float currentValue, float maxValue)
	{
		return currentValue / maxValue;
	}

	private void SetupThreshhold(int i)
	{
		//Debug.Log("Creating Threshold number " + i);
		GameObject threshhold = CreateNewThreshhold();

		UpdateThreshhold(threshhold, i);
		threshholds.Add(threshhold);
	}

	private GameObject CreateNewThreshhold()
	{
		return Instantiate(threshholdPrefab, threshholdContainer);
	}

	private void SetupJourneyBar(int i)
	{
		Debug.Log("Creating journey bar number " + i);
		
		JourneyEnergyBar journeyEnergyBar = CreateNewEnergyBar();
		UpdateJourneyBar(journeyEnergyBar, i);
		journeyEnergyBars.Add(journeyEnergyBar);
	}

	Color GetRandomColor(float alpha)
	{
		return new Color(Random.value, Random.value, Random.value, alpha);
	}

	void ApplyColor(Color col)
	{
		currentColor = col;

		if (playerAvatar != null)
			playerAvatar.color = col;

		if (currentRunEnergyBar != null)
			currentRunEnergyBar.GetComponent<Image>().color = col;
	}

	public JourneyEnergyBar CreateNewEnergyBar()
	{
		ApplyColor(GetRandomColor(0.75f));
		return new JourneyEnergyBar(currentColor, Instantiate(journeyRunEnergyBarPrefab, journeyContainer));
	}
}
