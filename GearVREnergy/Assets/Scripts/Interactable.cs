using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour {

	[SerializeField] private UnityEvent powerOnEvents;
	[SerializeField] private UnityEvent powerOffEvents;
	public bool isPowered = false;
	public bool badPowerState;


	public float energyConsumption = 50f;
	//public KeyCode changecol;

	void Start()
	{
	    CallPowerEvents();
		
	}	
	void CallPowerEvents()
	{
		if(isPowered)
		{
			powerOnEvents.Invoke();
		}
		else
		{
			powerOffEvents.Invoke();
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			isPowered = !isPowered;
			CallPowerEvents();
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
