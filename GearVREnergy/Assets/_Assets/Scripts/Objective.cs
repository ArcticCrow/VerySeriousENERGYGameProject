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
	public string propertyName;
	public string targetValue;

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

		Type componentType = component.GetType();
		PropertyInfo property = componentType.GetProperty(propertyName);
		Debug.Log(propertyName + "\n" + property);

		//Type propertyType = property.GetType();
		var propertyValue = property.GetValue(component, null);
		var desiredValue = Convert.ChangeType(targetValue, property.PropertyType);

		Debug.Log("Property Value: " + propertyValue.ToString()
			+ "\nDesired Value: " + desiredValue.ToString()
			+ "\nAre the same? " + propertyValue.Equals(desiredValue));

		return false;
	}
}

[Serializable]
//[CreateAssetMenu(fileName = "Player Objective", menuName = "Gameplay/Objective")]
public class Objective {
	// Things The player has to do before the objective is 'completed'
	public List<Condition> conditions = new List<Condition>();


	private void Awake()
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			conditions[i].IsConditionMet();
		}
	}

	public bool IsObjectiveCleared()
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (!conditions[i].IsConditionMet())
			{
				return false;
			}
		}

		return true;
	}
}
