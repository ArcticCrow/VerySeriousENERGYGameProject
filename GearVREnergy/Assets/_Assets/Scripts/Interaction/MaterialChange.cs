using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

	public Color on;
	public Color off;
	//public bool isPowered;
	public List<MeshRenderer> meshRenderers;
    public AudioClip aClip;
    public AudioSource aSource;
    // Use this for initialization


    public void Start()
    {
		if (aSource != null) aSource.clip = aClip;
    }
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
				if (aSource != null) aSource.Play();
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
		}
	}
}
