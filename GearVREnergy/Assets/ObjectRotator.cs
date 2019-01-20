using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour {

	public List<Transform> targets;
	public Quaternion rotationA;
	public Quaternion rotationB;
	public float rotationTime = 1f;

	public void RotateToA(bool overrideRotation)
	{
		if (overrideRotation) SetAllRotation(rotationB);
		StartCoroutine(Rotation(rotationA, overrideRotation, rotationB));
	}

	public void RotateToB(bool overrideRotation)
	{
		if (overrideRotation) SetAllRotation(rotationA);
		StartCoroutine(Rotation(rotationB, overrideRotation, rotationA));
	}

	public void SetAllRotation (Quaternion rotation)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			Transform target = targets[i];
			SetRotation(target, rotation);
		}
	}

	public void SetRotation(Transform target, Quaternion rotation)
	{
		target.localRotation = rotation;
	}

	public IEnumerator Rotation(Quaternion targetRotation, bool useStartRotation, Quaternion startRotation)
	{
		float elapsedTime = 0;
		while (elapsedTime < rotationTime)
		{
			elapsedTime += Time.deltaTime;
			for (int i = 0; i < targets.Count; i++)
			{
				Quaternion rot = (useStartRotation) ? startRotation : targets[i].localRotation;
				SetRotation(targets[i], Quaternion.Slerp(rot, targetRotation, elapsedTime / rotationTime));
			}
			yield return new WaitForEndOfFrame();
		}


		yield return null;
	}
}
