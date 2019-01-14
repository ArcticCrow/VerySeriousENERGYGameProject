using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
	public static BGMController instance;

	[Serializable]
	public struct BGMAudioClip
	{
		public AudioClip tutorial;
		public AudioClip core;
		public AudioClip insane;
		public AudioClip celebration;
	}
	public BGMAudioClip musicClips;

	AudioSource musicAudioSource;
	public float pitchSmoothingTime = 1f;

	private void Awake()
	{
		instance = this;
		musicAudioSource = GetComponent<AudioSource>();
	}

	public void PlayTutorialBGM()
	{
		PlayBGM(musicClips.tutorial);
	}

	public void PlayCoreBGM()
	{
		PlayBGM(musicClips.core);
	}

	public void PlayInsaneBGM()
	{
		PlayBGM(musicClips.insane);
	}

	public void PlayCelebrationBGM()
	{
		PlayBGM(musicClips.celebration);
	}

	private void PlayBGM(AudioClip bgmClip)
	{
		if (bgmClip == null)
		{
			Debug.LogAssertion("BGM Clip can't be null!");
			return;
		}
		musicAudioSource.clip = bgmClip;
		musicAudioSource.loop = true;
		musicAudioSource.Play();
	}

	public float GetCurrentClipLength()
	{
		if (musicAudioSource.clip == null)
		{
			Debug.LogAssertion("AI Clip can't be null!");
			return 0;
		}
		return musicAudioSource.clip.length;
	}

	public void ChangePitch(float pitch)
	{
		musicAudioSource.pitch = Mathf.Clamp(pitch, -3, 3);
	}

	public void FadeToPitch(float targetPitch)
	{
		StartCoroutine(PitchFade(targetPitch));
	}

	private IEnumerator PitchFade(float targetPitch)
	{
		float timeElapsed = 0;

		while (timeElapsed < pitchSmoothingTime)
		{
			ChangePitch(Mathf.Lerp(musicAudioSource.pitch, targetPitch, timeElapsed / pitchSmoothingTime));
			yield return new WaitForEndOfFrame();
			timeElapsed += Time.deltaTime;
		}
		print("Completed BGM pitch change.");
		yield return null;
	}
}
