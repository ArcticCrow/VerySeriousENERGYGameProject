using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class Throw : MonoBehaviour {

	float throwForce = 800f;
	Vector3 objectPos;
	float distance;
	public bool canHold = true;
	public GameObject item;
	public GameObject tempParent;
	public bool isHolding = false;
    private Rigidbody rb;
    public Vector3 offset;
   
    private void Start()
    {
        if(item == null)
        {
            item = gameObject;
        }
        rb = item.GetComponent<Rigidbody>();
    }
    void Update()
    {
        RaycastHit hit;
        if (isHolding == true && (OVRInput.GetDown(OVRInput.Button.One) || Input.GetMouseButtonDown(0)))
        {
            ReleaseObject();
        }
        else if (isHolding == false && Physics.Raycast(GameManager.instance.Pointer.position, GameManager.instance.Pointer.forward, out hit) && hit.transform == transform)
        {
            if ((OVRInput.GetDown(GameManager.instance.interactionButton) || Input.GetKeyDown(GameManager.instance.interactionKey)))
            {
                PickUpObject();
            }
        }

        
    }
	void PickUpObject()
	{
		isHolding = true;
        rb.isKinematic = true;
        item.transform.SetParent(GameManager.instance.Pointer);
        item.transform.position = offset;


    }
    void ReleaseObject()
	{
		isHolding = false;
        rb.isKinematic = false;
        item.transform.SetParent(null);
        rb.AddForce(tempParent.transform.forward * throwForce);
    }

}
