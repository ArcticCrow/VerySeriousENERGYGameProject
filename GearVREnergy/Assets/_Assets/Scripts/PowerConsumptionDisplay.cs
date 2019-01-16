using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerConsumptionDisplay : MonoBehaviour {

	public void UpdateColor(Color col)
	{
		Material mat = gameObject.GetComponentInChildren<MeshRenderer>().material;
		mat.color = col;
	}
}
