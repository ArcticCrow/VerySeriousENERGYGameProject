using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceVoice : SequenceStep {

	public SFXClip voiceClip = SFXClip.Voice_Tut_1;
	public bool useVoiceClipLength = false;

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
		SoundController.PlaySound(voiceClip);
		if (useVoiceClipLength)
		{
			waitTime = SoundController.GetSoundLength(voiceClip);
		}
		hasLaunched = true;
	}

	public override bool StepIsFinished()
	{
		return hasLaunched;
	}
}
