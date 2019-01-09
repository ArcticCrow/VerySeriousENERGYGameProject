using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXClip
{
    Door,
    Teleportation,
	Voice_Intro
}

public class SoundControl : MonoBehaviour
{
    public static AudioClip Door, Teleportation;
    static AudioSource audioSrc;

    

    void Start()
    {
        //List of all the audio sources being defined
        Door = Resources.Load<AudioClip>("Door");
        Teleportation = Resources.Load<AudioClip>("Teleportation");

        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(SFXClip clip)
    {
        switch (clip)
        {
            case SFXClip.Door:
                audioSrc.PlayOneShot(Door);
                break;

			case SFXClip.Teleportation:
				audioSrc.PlayOneShot(Teleportation);
				break;

			case SFXClip.Voice_Intro:
				audioSrc.PlayOneShot(Teleportation);
				break;
		}
        
    }
}
