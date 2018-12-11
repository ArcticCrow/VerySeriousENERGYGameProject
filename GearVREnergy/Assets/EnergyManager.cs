using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour {

	[Header("Initial Properties")]
	[SerializeField, Tooltip("A list of all objects that the AI should look to manipulate.")]
	private List<string> interactableObjectTags = new List<string>()
	{
		"Interactable"
	};
	[SerializeField, Tooltip("Listing of all found interactable objects in scene. Will be generated on scene start.")]
	List<GameObject> interactables;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
