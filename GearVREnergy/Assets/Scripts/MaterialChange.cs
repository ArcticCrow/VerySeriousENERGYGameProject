using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

	public Color on;
	public Color off;
	//public bool isPowered;
	MeshRenderer meshRenderer;
	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	public void PowerOn () {
		
       meshRenderer.material.color = on;
		
	}

	public void PowerOff()
	{
		
       meshRenderer.material.color = off;
	}
}
