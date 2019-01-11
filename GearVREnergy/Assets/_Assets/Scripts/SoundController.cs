using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXClip
{
    Door,
    Teleportation,

	Voice_Tut_1,
	Voice_Tut_2,
	Voice_Tut_3,
	Voice_Tut_4,
	Voice_Tut_5,
	Voice_Tut_6,
	Voice_Tut_7,
	Voice_Tut_8,
	Voice_Tut_9,

	Voice_Engine,
	Voice_Fridge,
	Voice_Heater,
	Voice_Kettle,
	Voice_LightSwitch,
	Voice_Microwave,
	Voice_Oven,
	Voice_Oxygen,
	Voice_Shower,
	Voice_Sink,
	Voice_Stereo,
	Voice_TV,
}

public class SoundController : MonoBehaviour
{
    public static AudioClip Door, Teleportation;
	public static Dictionary<SFXClip, AudioClip> tutorialClips;
	public static Dictionary<SFXClip, AudioClip> gameplayClips;
	
	static AudioSource audioSrc;
    

    void Start()
    {
		string path = "SFX/";

        //List of all the audio sources being defined
        //Door = Resources.Load<AudioClip>("Door");
        //Teleportation = Resources.Load<AudioClip>("Teleportation");

		#region AI Voice lines
		// Tutorial
		tutorialClips = new Dictionary<SFXClip, AudioClip>();
		tutorialClips.Add(SFXClip.Voice_Tut_1, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart1"));
		tutorialClips.Add(SFXClip.Voice_Tut_2, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart2"));
		tutorialClips.Add(SFXClip.Voice_Tut_3, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart3"));
		tutorialClips.Add(SFXClip.Voice_Tut_4, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart4"));
		tutorialClips.Add(SFXClip.Voice_Tut_5, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart5"));
		tutorialClips.Add(SFXClip.Voice_Tut_6, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart6"));
		tutorialClips.Add(SFXClip.Voice_Tut_7, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart7"));
		tutorialClips.Add(SFXClip.Voice_Tut_8, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart8"));
		tutorialClips.Add(SFXClip.Voice_Tut_9, Resources.Load<AudioClip>(path + "AI Voice/TutorialPart9"));

		// Gameplay
		gameplayClips = new Dictionary<SFXClip, AudioClip>();
		gameplayClips.Add(SFXClip.Voice_Engine, Resources.Load<AudioClip>(path + "AI Voice/Engine"));
		gameplayClips.Add(SFXClip.Voice_Fridge, Resources.Load<AudioClip>(path + "AI Voice/Fridge"));
		gameplayClips.Add(SFXClip.Voice_Heater, Resources.Load<AudioClip>(path + "AI Voice/Heater"));
		gameplayClips.Add(SFXClip.Voice_Kettle, Resources.Load<AudioClip>(path + "AI Voice/Kettle"));
		gameplayClips.Add(SFXClip.Voice_LightSwitch, Resources.Load<AudioClip>(path + "AI Voice/LightSwitch"));
		gameplayClips.Add(SFXClip.Voice_Microwave, Resources.Load<AudioClip>(path + "AI Voice/Microwave"));
		gameplayClips.Add(SFXClip.Voice_Oven, Resources.Load<AudioClip>(path + "AI Voice/Oven"));
		gameplayClips.Add(SFXClip.Voice_Oxygen, Resources.Load<AudioClip>(path + "AI Voice/Oxygen"));
		gameplayClips.Add(SFXClip.Voice_Shower, Resources.Load<AudioClip>(path + "AI Voice/Shower"));
		gameplayClips.Add(SFXClip.Voice_Sink, Resources.Load<AudioClip>(path + "AI Voice/Sink"));
		gameplayClips.Add(SFXClip.Voice_Stereo, Resources.Load<AudioClip>(path + "AI Voice/Stereo"));
		gameplayClips.Add(SFXClip.Voice_TV, Resources.Load<AudioClip>(path + "AI Voice/TV"));

		#endregion AI Voice lines

		audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(SFXClip clip)
    {
		if (clip.ToString().Contains("Voice_Tut"))
		{
			if (tutorialClips[clip] == null)
			{
				Debug.LogAssertion("Tutorial Clip " + clip.ToString() + " couldn't be found!");
				return;
			}
			audioSrc.PlayOneShot(tutorialClips[clip]);
			//Debug.Log("Length of " + clip.ToString() + ": " + tutorialClips[clip].length);
		}
		else if (clip.ToString().Contains("Voice_"))
		{
			if (gameplayClips[clip] == null)
			{
				Debug.LogAssertion("Gameplay Clip " + clip.ToString() + " couldn't be found!");
				return;
			}
			audioSrc.PlayOneShot(gameplayClips[clip]);
			//Debug.Log("Length of " + clip.ToString() + ": " + gameplayClips[clip].length);
		}
		else
		{
			Debug.LogAssertion("Clip " + clip.ToString() + " couldn't be found!");
		}
	}

	public static float GetSoundLength (SFXClip clip)
	{

		if (clip.ToString().Contains("Voice_Tut"))
		{
			if (tutorialClips[clip] == null)
			{
				Debug.LogAssertion("Tutorial Clip " + clip.ToString() + " couldn't be found!");
				return 0;
			}
			return tutorialClips[clip].length;
		}
		else if (clip.ToString().Contains("Voice_"))
		{
			if (gameplayClips[clip] == null)
			{
				Debug.LogAssertion("Gameplay Clip " + clip.ToString() + " couldn't be found!");
				return 0;
			}

			return gameplayClips[clip].length;
		}
		else
		{
			Debug.LogAssertion("Clip " + clip.ToString() + " couldn't be found!");
			return 0;
		}
	}
}
