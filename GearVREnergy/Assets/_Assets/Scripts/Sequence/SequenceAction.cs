using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceAction : SequenceStep
{
    public UnityEvent actions;
    public override bool StepIsFinished()
    {
        if (actions != null)
        {
            actions.Invoke();
        } 
        else
        {
            Debug.LogAssertion("No actions to invoke!");
        }
        
        return true;
    }
}
