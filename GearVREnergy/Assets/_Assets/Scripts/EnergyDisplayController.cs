using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDisplayController : MonoBehaviour {

	public List<RectTransform> totalEnergyDisplays;
	public List<RectTransform> positiveFluctuationDisplays;
	public List<RectTransform> negativeFluctuationDisplays;

	[SerializeField]
	float energyPercent = 0;
	[SerializeField]
	float positivePercent = 0;
	[SerializeField]
	float negativePercent = 0;

	private void Update()
	{
		float newEnergyPercent = CalculatePercentWithDecimalPlaces(EnergyManager.instance.currentEnergy, EnergyManager.instance.maxEnergy, 2);
		float newPositivePercent = CalculatePercentWithDecimalPlaces(EnergyManager.instance.positiveInfluencePerTick, 1000f/EnergyManager.instance.ticksPerSecond, 2);
		float newNegativePercent = CalculatePercentWithDecimalPlaces(EnergyManager.instance.negativeInfluencePerTick, 1000f/EnergyManager.instance.ticksPerSecond, 2);

		if (newEnergyPercent != energyPercent)
		{
			energyPercent = newEnergyPercent;
			for (int i = 0; i < totalEnergyDisplays.Count; i++)
			{
				Vector3 scale = totalEnergyDisplays[i].localScale;
				scale.y = energyPercent;
				totalEnergyDisplays[i].localScale = scale;
			}
		}
		if (newPositivePercent != positivePercent)
		{
			positivePercent = newPositivePercent;
			for (int i = 0; i < positiveFluctuationDisplays.Count; i++)
			{
				Vector3 scale = positiveFluctuationDisplays[i].localScale;
				scale.y = positivePercent;
				positiveFluctuationDisplays[i].localScale = scale;
			}
		}
		if (newNegativePercent != negativePercent)
		{
			negativePercent = newNegativePercent;
			for (int i = 0; i < negativeFluctuationDisplays.Count; i++)
			{
				Vector3 scale = negativeFluctuationDisplays[i].localScale;
				scale.y = negativePercent;
				negativeFluctuationDisplays[i].localScale = scale;
			}
		}
	}

	private float CalculatePercentWithDecimalPlaces(float value, float maxValue, int decimalPlaces)
	{
		if (decimalPlaces < 0) decimalPlaces = 0;
		return Mathf.Round(Mathf.Clamp01(value / maxValue) * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
	}
}
