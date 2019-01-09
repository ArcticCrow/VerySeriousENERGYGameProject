using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
//[CreateAssetMenu(fileName = "Player Objective", menuName = "Gameplay/Objective")]
public class SequenceObjective : SequenceStep {

    public enum Comparison
    {
        AND,
        OR
    }

    public Comparison comparisonOperator;

    // Things The player has to do before the objective is 'completed'
    public List<Condition> conditions = new List<Condition>();

	public override bool StepIsFinished()
	{
		return IsObjectiveCleared();
	}

	public bool IsObjectiveCleared()
	{
        switch (comparisonOperator)
        {
            case Comparison.AND:
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (!conditions[i].IsConditionMet())
                    {
                        return false;
                    }
                }
                return true;

            case Comparison.OR:
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i].IsConditionMet())
                    {
                        return true;
                    }
                }
                return false;
        }

        Debug.LogAssertion("Something went wrong when checking an objective.");
        return false;
    }
}
