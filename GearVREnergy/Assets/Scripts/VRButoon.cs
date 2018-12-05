using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VRButoon : MonoBehaviour {

	public Image bgImage;
	public Color NormalColor;
	public Color HighLightColor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void OnGazeEnter()
	{
		bgImage.color = HighLightColor;
	}
	public void OnGazeExit()
	{
		bgImage.color = 	NormalColor;
	}
	public void OnClick()
	{
		Debug.Log("Horraay it's working!");
	}
}
