using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour {

	public static SFXController instance;

	[Serializable]
	public struct SFXAudioClip
	{
		public AudioClip door;
		public AudioClip teleportation;
		public AudioClip shenanigans;
	}
	public SFXAudioClip soundEffects;

	static AudioSource audioSource;

	private void Awake()
	{
		instance = this;
		audioSource = GetComponent<AudioSource>();
	}

	public static void PlaySound(AudioClip clip, float volume = 1f)
	{
		if (clip == null)
		{
			Debug.LogAssertion("SFX Clip can't be null!");
			return;
		}
		audioSource.PlayOneShot(clip, volume);
	}

	public static void StopSounds()
	{
		audioSource.Stop();
	}
}
