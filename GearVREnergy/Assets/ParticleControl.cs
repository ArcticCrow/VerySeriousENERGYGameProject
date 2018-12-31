using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour
{

    ParticleSystem system;

    public void StartSystem()
    {
        if (CheckSystem())
        {
            if (!system.isPlaying)
            {
                system.Play();
            }
        }
    }
    public void StopSystem()
    {
        if (CheckSystem())
        {
            if (system.isPlaying && !system.isStopped)
            {
                system.Stop();
            }
        }
    }

    public bool CheckSystem()
    {
        if (system == null)
        {
            system = GetComponent<ParticleSystem>();
        }
        if (system == null)
        {
            Debug.LogWarning("No particle system attached!");
            return false;
        }
        return true;
    }
}