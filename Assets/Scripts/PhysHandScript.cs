using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysHandScript : MonoBehaviour
{

    private FixedJoint _GrabJoint;
    [SerializeField]
    private HandScript _ParentHand;
    [SerializeField]
    private LayerMask GrabMask;
    [SerializeField]
    private int side = 1;

    [SerializeField]
    private Transform _ShootSpot;
    [SerializeField]
    private float _Range;

    [SerializeField]
    private LayerMask _DynamicObjectLayerMask;

    [SerializeField]
    private LayerMask _NotDynamicObject;

    [SerializeField]
    private Animator _HandAnimator;
    [SerializeField]
    private Animator _ReticleAnimator;

    private bool _shooting;

    private void Start()
    {
        //_GrabJoint = GetComponent<FixedJoint>();
    }

    private void Update()
    {
        TargetStatus();
        Grab();
    }

    private void TargetStatus()
    {
        RaycastHit _TargetInfo;
        if (Physics.Raycast(_ShootSpot.position, _ShootSpot.forward, out _TargetInfo, _Range, _DynamicObjectLayerMask))
        {
            //Debug.Log("Dynamic");
            _ReticleAnimator.SetInteger("RangeStatus", 1);
            _ReticleAnimator.SetBool("isObject", true);
        }
        else if (Physics.Raycast(_ShootSpot.position, _ShootSpot.forward, out _TargetInfo, _Range, _NotDynamicObject))
        {
            //Debug.Log("Not Dynamic");
            _ReticleAnimator.SetInteger("RangeStatus", 1);
            _ReticleAnimator.SetBool("isObject", false);
        }
        else
        {
            //Debug.Log("nothing");
            _ReticleAnimator.SetInteger("RangeStatus", 0);
            _ReticleAnimator.SetBool("isObject", false);
        }
    }

    private void Grab()
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
                _HandAnimator.SetInteger("GrabType", 1);
            }
            else
            {
                _HandAnimator.SetInteger("GrabType", 0);
            }
            _HandAnimator.SetBool("Grabbing", true);


            Debug.DrawRay(transform.position, side * transform.right, Color.red);
        }

        if (_ParentHand._Grab.GetStateUp(_ParentHand._handType))
        {
            _HandAnimator.SetBool("Grabbing", false);
            Destroy(_GrabJoint);
        }
    }
}
