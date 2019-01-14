using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceAudio : SequenceStep {

	public AudioClip audioClip;
	public bool useAudioClipLength = false;

	public void OnValidate()
	{
		type = Type.Voice;
	}

	public override void Complete()
	{
		hasCompleted = true;
	}

	public override void Launch()
	{
		if (audioClip == null)
		{
			Debug.LogWarning("No audio clip was set!");
		}
		else
		{
			SFXController.PlaySound(audioClip);
			if (useAudioClipLength)
			{
				waitTime = audioClip.length;
			}
		}
		hasLaunched = true;
	}

	public override bool StepIsFinished()
	{
		return hasLaunched;
	}
}
