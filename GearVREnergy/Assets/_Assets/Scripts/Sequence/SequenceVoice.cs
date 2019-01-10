using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceVoice : SequenceStep {

	public SFXClip voiceClip = SFXClip.Voice_Tut_1;
	public bool useVoiceClipLength = false;

	public override void Complete()
	{
		hasCompleted = true;
	}

	public override void Launch()
	{
		SoundControl.PlaySound(voiceClip);
		if (useVoiceClipLength)
		{
			waitTime = SoundControl.GetSoundLength(voiceClip);
		}
		hasLaunched = true;
	}

	public override bool StepIsFinished()
	{
		return hasLaunched;
	}
}
