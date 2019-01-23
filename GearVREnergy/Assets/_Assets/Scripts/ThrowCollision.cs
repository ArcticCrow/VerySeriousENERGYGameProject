using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowCollision : MonoBehaviour 
{
	public bool activateCollisionSound = false;

	private void OnCollisionEnter(Collision collision)
	{
		if (activateCollisionSound)
		{
			SFXController.PlaySound(SFXController.instance.soundEffects.collision, 0.75f);
		}
	}
}