using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	public Material material;
	public Color newColor;
	public bool isPowered;
	//public KeyCode changecol;

	void Start()
	{
		material.color = Color.white;
	}	

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (material.color == Color.red)
			{
				material.color = newColor;
				Debug.Log("Lights are On");
				isPowered = true;
			}
			else if(material.color != Color.red) {
				material.color = Color.red;
				Debug.Log("Lights are Off");
				isPowered = false;
			}
		}
	}
/*
public Material[]materials;
public Renderer renderer;
public bool isPowered;

private int index = 1;

void Start()
{
	renderer = GetComponent<Renderer> ();
	renderer.enabled = true;
}

void OnMouseDown()
{    
	
	if(materials.Length == 0)
	{
		return;
	}
	if (Input.GetMouseButtonDown (0))
	{
		index += 1;
		Debug.Log("Lights are Off");
		isPowered = false;
		

		if(index == materials.Length +1)
		{
			index = 1;
			Debug.Log("Lights are On");
			isPowered = true;
			
		}
		//print (index);
			renderer.sharedMaterial = materials [index-1];
	}
	
}
*/

}
