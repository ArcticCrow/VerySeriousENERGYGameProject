using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVoiceController : MonoBehaviour {

	public static AIVoiceController instance;
	static AudioSource audioSource;

	private void Awake()
	{
		instance = this;
		audioSource = GetComponent<AudioSource>();
	}

	public static void Play(AudioClip clip)
	{
		if (clip == null)
		{
			Debug.LogAssertion("AI Audio Clip can't be null!");
			return;
		}
		audioSource.clip = clip;
		audioSource.Play();
	}

	public static void StopSounds()
	{
		audioSource.Stop();
	}
}
