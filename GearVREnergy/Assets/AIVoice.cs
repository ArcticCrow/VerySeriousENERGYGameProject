using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVoice : SequenceStep {

	public SFXClip voiceClip = SFXClip.Voice_Intro;
	public bool waitForFinish = false;

	public override bool StepIsFinished()
	{
		SoundControl.PlaySound(voiceClip);
		return true;
	}
}
