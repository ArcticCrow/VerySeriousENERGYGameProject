using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

	float throwForce = 600;
	Vector3 objectPos;
	float distance;

	public bool canHold = true;
	public GameObject item;
	public GameObject tempParent;
	public bool isHolding = false;


    void Update()
    {
        //RaycastHit hit;
        //if (Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform == transform)
        //{

            if (isHolding == true)
            {
                item.GetComponent<Rigidbody>().velocity = Vector3.zero;
                item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                item.transform.SetParent(tempParent.transform);

                if (Input.GetMouseButtonDown(1))
               // if ((OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(GameManager.instance.interactionKey)))
                {
                    item.GetComponent<Rigidbody>().AddForce(tempParent.transform.forward * throwForce);
                }
            }
            else
            {
                objectPos = item.transform.position;
                item.transform.SetParent(null);
                item.GetComponent<Rigidbody>().useGravity = true;
                item.transform.position = objectPos;

            }
            /*
           if(Input.GetMouseButtonDown(0))
            {
                isHolding = true;
                item.GetComponent<Rigidbody>().useGravity = false;
                item.GetComponent<Rigidbody>().detectCollisions = false;
            }
            */

        //}
    }
    
	void OnMouseDown()
	{
		isHolding = true;
		item.GetComponent<Rigidbody>().useGravity = false;
		item.GetComponent<Rigidbody>().detectCollisions = false;
		
	}
    
    
    

	void OnMouseUp()
	{
		isHolding = false;
	}

}
