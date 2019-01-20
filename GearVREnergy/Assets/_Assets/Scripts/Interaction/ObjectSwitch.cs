using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSwitch : MonoBehaviour {

    public GameObject target;

    public void SwitchOn()
    {
        if (CheckTarget())
        {
            target.SetActive(true);
        }
    }
    public void SwitchOff()
    {
        if (CheckTarget())
        {
			target.SetActive(false);
        }
    }

    private bool CheckTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("No target set!");
            return false;
        }
        return true;
    }
}
