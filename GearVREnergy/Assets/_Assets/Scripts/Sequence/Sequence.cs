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
		Action
	}
	public Type type;

	public bool waitAfterLaunch = false;
	public float launchWaitTime = 0;

	public bool waitAfterCompletion = false;
    public float completionWaitTime = 0;

	[HideInInspector]
	public bool hasLaunched = false;
	[HideInInspector]
	public bool hasCompleted = false;

	public abstract void Launch();
	public abstract bool StepIsFinished();
	public abstract void Complete();
	public abstract void Stop();
}

[Serializable]
public class Sequence : MonoBehaviour{
	public List<SequenceStep> steps;

	int currentStep = 0;
	bool isPlaying = false;

	[HideInInspector]
	public bool isSequenceFinished = false;

	public void LaunchSequence()
	{
		if (!isPlaying)
		{
			StartCoroutine(PlaySequence());
		}
	}

	public void StopSequence()
	{
		if (isPlaying)
		{
			StopCoroutine(PlaySequence());
			isPlaying = false;
		}
		for (int i = 0; i < steps.Count; i++)
		{
			steps[i].Stop();
		}
	}

	IEnumerator PlaySequence()
	{
		isPlaying = true;

        while (currentStep < steps.Count)
        {
            SequenceStep step = steps[currentStep];

			if (step != null)
			{
				step.Launch();
				do
				{
					yield return null;
				}
				while (!step.StepIsFinished());

				if (step.waitAfterLaunch)
				{
					yield return new WaitForSeconds(step.launchWaitTime);
				}

				step.Complete();

				if (step.waitAfterCompletion)
				{
					yield return new WaitForSeconds(step.completionWaitTime);
				}

				while (!step.hasCompleted) yield return null;
			}

            currentStep++;
            Debug.Log("Step '" + step.gameObject.name + "' is complete!");
        }

        isSequenceFinished = true;
        Debug.Log("Sequence '" + name + "' is finished!");

		StopSequence();

		yield return null;
	}
}
