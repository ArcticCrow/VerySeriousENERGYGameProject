using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceVoice : SequenceStep {

	public SFXClip voiceClip = SFXClip.Voice_Intro;

	public override bool StepIsFinished()
	{
		SoundControl.PlaySound(voiceClip);
		return true;
	}
}
