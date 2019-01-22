using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

	public Color on;
	public Color off;
	//public bool isPowered;
	public List<MeshRenderer> meshRenderers;
    public AudioClip aClip;
    
    // Use this for initialization


   
    void CheckRenderer () {
		if (meshRenderers == null || meshRenderers.Count == 0)
		{
			meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
			meshRenderers.Add(GetComponent<MeshRenderer>());
		}
	}
	
	// Update is called once per frame
	public void PowerOn ()
    {
		//print(gameObject.name + ": MaterialChange: Powering on");
		CheckRenderer();
		for (int i = 0; i < meshRenderers.Count; i++)
		{
			if (meshRenderers[i] != null)
				meshRenderers[i].material.color = on;
			    SFXController.PlaySound(aClip, 0.6f);
		}
	}

	public void PowerOff()
	{
		//print(gameObject.name + ": MaterialChange: Powering off");
        CheckRenderer();
		for (int i = 0; i < meshRenderers.Count; i++)
		{
			if (meshRenderers[i] != null)
				meshRenderers[i].material.color = off;

            SFXController.PlaySound(aClip, 0.6f);
        }
	}
}
