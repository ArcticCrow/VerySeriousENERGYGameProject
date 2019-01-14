using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceAudio : SequenceStep {

	[Header("Main Audio")]
	public AudioClip audioClip;
	public bool useAudioClipLength = false;

	[Header("Reminders")]
	public bool useReminders = false;
	public bool playRemindersOnce = false;
	public bool waitOneInterval = true;
	public float reminderInterval = 15f;
	[Tooltip("The step after which the reminders stop playing")]
	public SequenceStep reminderStoppingStep;
	public List<AudioClip> reminderClips;

	public void OnValidate()
	{
		type = Type.Voice;
	}

	public override void Complete()
	{
		if (useReminders)
		{
			StartCoroutine(ReminderCoroutine());
		}
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
			AIVoiceController.Play(audioClip);
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

	IEnumerator ReminderCoroutine()
	{
		if (reminderClips != null)
		{
			//print("reminder waiting wait time");
			yield return new WaitForSeconds(waitTime);

			if (waitOneInterval)
			{
				//print("reminder waiting one interval");
				yield return new WaitForSeconds(reminderInterval);
			}

			List<AudioClip> reminders = new List<AudioClip>(reminderClips);

			//print("Reminder Count: " + reminderClips.Count + "\nStopping Step Status: " + IsStoppingStepFinished());
			while (reminders != null && !IsStoppingStepFinished())
			{
				//print("random reminder picking");
				int reminderIndex = Random.Range(0, reminderClips.Count);
				AudioClip reminder = reminders[reminderIndex];
				if (playRemindersOnce)
				{
					//print("removing picked reminder " + reminder.name);
					reminders.Remove(reminder);
				}
				//print("Stopping Step Status: " + IsStoppingStepFinished());
				if (IsStoppingStepFinished())
					break;

				//print("Playing reminder");
				AIVoiceController.Play(reminder);

				//print("Waiting interval");
				yield return new WaitForSeconds(reminderInterval + reminder.length);
			}
			//print("Stopping all sfx sounds");
			AIVoiceController.StopSounds();
		}
		else
		{
			Debug.LogWarning("No reminder clips have been assigned!");
		}

		//print("reminder finished " + gameObject.name);
		yield return null;
	}

	private bool IsStoppingStepFinished()
	{
		return reminderStoppingStep == null || (reminderStoppingStep != null && reminderStoppingStep.StepIsFinished());
	}
}
