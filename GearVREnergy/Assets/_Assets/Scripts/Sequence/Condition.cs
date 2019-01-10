using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Condition : MonoBehaviour
{
	[HideInInspector]
	public bool isInitialized = false;
	public abstract void Initialize();
    public abstract bool IsConditionMet();
	public abstract void Finish();
}