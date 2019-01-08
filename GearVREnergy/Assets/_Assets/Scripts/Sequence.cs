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
		Transition
	}
	public Type type;

	public abstract bool StepIsFinished();
}

[Serializable]
public class Sequence : MonoBehaviour{
	public List<SequenceStep> steps = new List<SequenceStep>();

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
		yield return null;
	}
}
