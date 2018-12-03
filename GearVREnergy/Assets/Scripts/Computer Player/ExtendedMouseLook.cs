using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedMouseLook : MouseLook {

	[Header("Custom Settings")]
	public bool useHotkeyToUnlockCursor = false;
	public KeyCode hotkey = KeyCode.LeftControl;

	// Update is called once per frame
	protected override void Update () {
		if (useHotkeyToUnlockCursor == true && Input.GetKey(hotkey))
		{
			// enable UI interaction
			
		}
		else
		{
			base.Update();
		}
	}
}
