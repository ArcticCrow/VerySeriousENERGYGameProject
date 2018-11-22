using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedMouseLook : MouseLook {

	[Header("Custom Settings")]
	public bool useHotkey = false;
	public KeyCode hotkey = KeyCode.LeftControl;

	// Update is called once per frame
	protected override void Update () {
		if (useHotkey == false || Input.GetKey(hotkey))
		{
			base.Update();
		}
	}
}
