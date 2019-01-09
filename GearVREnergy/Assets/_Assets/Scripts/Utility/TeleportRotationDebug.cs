using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportRotationDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
