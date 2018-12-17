using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleControlPanel : MonoBehaviour {

	public List<GameObject> controls;

	public void TurnControlsOn()
	{
		for (int i = 0; i < controls.Count; i++) {
			controls[i].SetActive(true);
		}
	}
	public void TurnControlsOff()
	{
		for (int i = 0; i < controls.Count; i++) {
			controls[i].SetActive(false);
		}
	}
}
