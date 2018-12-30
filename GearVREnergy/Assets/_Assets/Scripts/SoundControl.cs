using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Door":
                audioSrc.PlayOneShot(Door);
                break;
        }
        switch (clip)
        {
            case "Teleportation":
                audioSrc.PlayOneShot(Teleportation);
                break;
        }
        
    }
}
