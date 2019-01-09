using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
//[CreateAssetMenu(fileName = "Player Objective", menuName = "Gameplay/Objective")]
public class Objective : SequenceStep {
	// Things The player has to do before the objective is 'completed'
	public List<Condition> conditions = new List<Condition>();

	public override bool StepIsFinished()
	{
		return IsObjectiveCleared();
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
