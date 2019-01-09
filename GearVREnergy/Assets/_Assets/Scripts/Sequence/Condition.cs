using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Condition : MonoBehaviour
{
    public abstract bool IsConditionMet();
}