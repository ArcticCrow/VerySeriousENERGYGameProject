using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

	public Color on;
	public Color off;
	//public bool isPowered;
	MeshRenderer meshRenderer;
	// Use this for initialization
	void CheckRenderer () {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            //meshRenderer.material = new Material(meshRenderer.material);
        }
	}
	
	// Update is called once per frame
	public void PowerOn ()
    {
		//print(gameObject.name + ": MaterialChange: Powering on");
		CheckRenderer();
		meshRenderer.material.color = on;
	}

	public void PowerOff()
	{
		//print(gameObject.name + ": MaterialChange: Powering off");
        CheckRenderer();
        meshRenderer.material.color = off;
	}
}
