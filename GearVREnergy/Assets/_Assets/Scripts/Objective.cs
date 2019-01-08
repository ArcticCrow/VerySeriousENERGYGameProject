using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct Condition
{
	public GameObject gameObject;
	public Component component;
	public string parameterName;
	public string targetValue;
	public PropertyInfo property;

	public bool IsConditionMet()
	{
		if (component == null)
		{
			throw new Exception("Condition does not have a set component!");
		}
		if (gameObject == null)
		{
			gameObject = component.gameObject;
		}

		Type type = component.GetType();
		

		return false;
	}
}

[Serializable]
[CreateAssetMenu(fileName = "Player Objective", menuName = "Gameplay")]
public class Objective : ScriptableObject {
	// Things The player has to do before the objective is 'completed'
	public List<Condition> conditions = new List<Condition>();
	
}
