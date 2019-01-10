using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SequenceStep : MonoBehaviour
{
	public enum Type
	{
		Objective,
		Voice,
		Cinematic,
		Setup
	}
	public Type type;

    public bool waitAfterCompletion = false;
    public float waitTime = 0;

	[HideInInspector]
	public bool hasLaunched = false;
	[HideInInspector]
	public bool hasCompleted = false;

	public abstract void Launch();
	public abstract bool StepIsFinished();
	public abstract void Complete();
}

[Serializable]
public class Sequence : MonoBehaviour{
	public List<SequenceStep> steps;

	int currentStep = 0;
	bool isPlaying = false;

	[HideInInspector]
	public bool isSequenceFinished = false;

	public bool IsSequenceComplete()
	{
		for (int i = 0; i < steps.Count; i++)
		{
			if (!steps[i].StepIsFinished())
			{
				return false;
			}
		}

		return true;
	}

	public void LaunchSequence()
	{
		StartCoroutine(PlaySequence());
	}

	public void StopSequence()
	{
		StopCoroutine(PlaySequence());
		isPlaying = false;
	}

	IEnumerator PlaySequence()
	{
		isPlaying = true;

        while (currentStep < steps.Count)
        {
            SequenceStep step = steps[currentStep];
			step.Launch();
            do
            {
                yield return null;
            }
            while (!step.StepIsFinished());

			step.Complete();

            if (step.waitAfterCompletion)
            {
                yield return new WaitForSeconds(step.waitTime);
            }

			while (!step.hasCompleted) yield return null;

            currentStep++;
            Debug.Log(currentStep + ". Step completed");
        }

        isSequenceFinished = false;
        Debug.Log("Sequence completed");
		isPlaying = false;
		yield return null;
	}
}
