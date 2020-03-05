using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysHandScript : MonoBehaviour
{

    private FixedJoint _GrabJoint;
    public HandScript _ParentHand;

    public LayerMask GrabMask;
    public int side = 1;

    private void Start()
    {
        //_GrabJoint = GetComponent<FixedJoint>();
    }
    /*
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Je touche");
        if (_ParentHand._Grab.state)
        {
            Debug.Log("J'attrappe");
            if (collision.gameObject.tag == "Dynamic")
            {
                Debug.Log("On est là");
                _GrabJoint.connectedBody = collision.rigidbody;
            }
        }
    }*/

    private void Update()
    {
        if (_ParentHand._Grab.GetStateDown(_ParentHand._handType))
        {
            Debug.Log(gameObject.name + "attrappe");
            RaycastHit Grab;
            if (Physics.Raycast(transform.position, side * transform.right, out Grab, .1f, GrabMask))
            {
                if (_GrabJoint == null)
                {
                    _GrabJoint = gameObject.AddComponent<FixedJoint>();
                }
                _GrabJoint.connectedBody = Grab.rigidbody;
            }
            Debug.DrawRay(transform.position, side * transform.right, Color.red);
        }

        if (_ParentHand._Grab.GetStateUp(_ParentHand._handType))
        {
            Destroy(_GrabJoint);
        }
    }
}
