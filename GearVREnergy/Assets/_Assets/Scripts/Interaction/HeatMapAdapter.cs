using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapAdapter : MonoBehaviour {

	public List<HeatMap> heatmaps = new List<HeatMap>();

	// Use this for initialization
	void Start () {
		if (heatmaps.Count == 0)
		{
			Debug.LogWarning("No heatmaps were set, disabling adapter.");
			gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
