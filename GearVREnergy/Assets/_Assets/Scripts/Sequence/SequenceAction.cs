using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceAction : SequenceStep
{
    public UnityEvent actions;
	public UnityEvent actionsUponCompletion;

	public void OnValidate()
	{
		type = Type.Action;
	}

	public override void Complete()
	{
		if (actionsUponCompletion != null)
		{
			actionsUponCompletion.Invoke();
		}
		hasCompleted = true;
	}

	public override void Launch()
	{
		if (actions != null)
		{
			actions.Invoke();
		}
		else
		{
			Debug.LogAssertion("No actions to invoke!");
		}
		hasLaunched = true;
	}

	public override bool StepIsFinished()
    {
        return hasLaunched;
    }
}
