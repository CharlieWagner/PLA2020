using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysHandScript : MonoBehaviour
{
    
    [SerializeField]
    private HandScript _ParentHand;// Controller
    [SerializeField]
    private LayerMask GrabMask;    // Grabbing mask, excludes player
    [SerializeField]
    private int side = 1;          // the side of the hand

    private FixedJoint _GrabJoint; // the fixed joint used for grabbing stuff

    [SerializeField]
    private Transform _ShootSpot;  // origin of the shooting
    [SerializeField]
    private float _Range;          // Shooting Range
    [SerializeField]
    private float _ObjectAimSpread;// Spread of the bject Auto-Aim
    
    private GameObject _CurrentTarget; // Current target Object
    private Vector3 _AimPos;           // Current Target Position

    [SerializeField]
    private LayerMask _DynamicObjectLayerMask; // Mask for dynamic object targeting
    [SerializeField]
    private LayerMask _NotDynamicObject; // Mask for regular object targeting

    [SerializeField]
    private Animator _HandAnimator; // Hand mesh animator
    [SerializeField]
    private Animator _ReticleAnimator; // Reticle object animator
    [SerializeField]
    private LineRenderer _AimLine;      // Aiming line renderer

    private int _RangeStatus;            // Range Status, 0 no range, 1 in range, 2 shooting
    private bool _TargetIsObject;        // if target is object
    private SpringJoint _ObjectShootJoint; // Shooting Joint

    [SerializeField]
    private LineRenderer _ShootLine; // Shooting Line Renderer

    private bool _shooting;

    private void Start()
    {
        //_GrabJoint = GetComponent<FixedJoint>();
    }

    private void Update()
    {
        if (_RangeStatus != 2)
            TargetStatus();

        LineUpdate();
        ReticleUpdate();
        updateShootLine();

        Shoot();

        Grab();
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

    private void TargetStatus()
    {
        RaycastHit _TargetInfo;
        //if (Physics.Raycast(_ShootSpot.position, _ShootSpot.forward, out _TargetInfo, _Range, _DynamicObjectLayerMask))
        if (Physics.SphereCast(_ShootSpot.position, _ObjectAimSpread, _ShootSpot.forward, out _TargetInfo, _Range, _DynamicObjectLayerMask))
        {
            _RangeStatus = 1;
            _TargetIsObject = true;

            _CurrentTarget = _TargetInfo.collider.gameObject;

        }
        else if (Physics.Raycast(_ShootSpot.position, _ShootSpot.forward, out _TargetInfo, _Range, _NotDynamicObject))
        {
            _RangeStatus = 1;
            _TargetIsObject = false;

            _CurrentTarget = null;
        }
        else
        {
            _RangeStatus = 0;
            _TargetIsObject = false;

            _CurrentTarget = null;
        }
        _AimPos = _TargetInfo.point;
    }

    private void LineUpdate()
    {
        if (_RangeStatus == 1)
            _AimLine.SetPosition(1, _AimPos);
        else
            _AimLine.SetPosition(1, _AimLine.transform.position);

        _AimLine.SetPosition(0, _AimLine.transform.position);
    }

    private void ReticleUpdate()
    {
        _ReticleAnimator.SetInteger("RangeStatus", _RangeStatus);
        _ReticleAnimator.SetBool("isObject", _TargetIsObject);
    }

    private void Shoot()
    {
        if (_ParentHand._Shoot.GetStateDown(_ParentHand._handType) && _RangeStatus == 1) // Shoot press
        {
            switch(_TargetIsObject)
            {
                case true : //   Target is object

                    _ObjectShootJoint = gameObject.AddComponent<SpringJoint>();
                    _ObjectShootJoint.connectedBody = _CurrentTarget.GetComponent<Rigidbody>();
                    initObjectShootJoint(_ObjectShootJoint);

                    _RangeStatus = 2;
                    break;
                case false : //  Target is not object
                    _RangeStatus = 2;
                    break;
            }
        }

        if (_ParentHand._Shoot.GetStateUp(_ParentHand._handType) && _RangeStatus == 2) // Shoot release
        {
            switch (_TargetIsObject)
            {
                case true: //   Target is object
                    
                    Destroy(_ObjectShootJoint);
                    _CurrentTarget = null;

                    _RangeStatus = 0;
                    break;
                case false: //  Target is not object

                    _CurrentTarget = null;
                    _RangeStatus = 0;
                    break;
            }
        }
    }

    private void updateShootLine()
    {
        if (_RangeStatus == 2)
        {
            switch (_TargetIsObject)
            {
                case true: //   Target is object

                    _ShootLine.SetPosition(0, _ShootLine.transform.position);
                    _ShootLine.SetPosition(1, _ObjectShootJoint.connectedBody.transform.TransformPoint(_ObjectShootJoint.connectedAnchor));
                    break;
                case false: //  Target is not object
                    _ShootLine.SetPosition(0, _ShootLine.transform.position);
                    _ShootLine.SetPosition(1, _ShootLine.transform.position);
                    break;
            }
        }
        else
        {
            _ShootLine.SetPosition(0, _ShootLine.transform.position);
            _ShootLine.SetPosition(1, _ShootLine.transform.position);
        }
    }

    private void initObjectShootJoint(SpringJoint Joint) // -------------------------------------------- Object Shoot joint initialiser
    {
        Joint.autoConfigureConnectedAnchor = false;
        //Joint.connectedAnchor = _AimPos - _CurrentTarget.transform.position;

        Joint.connectedAnchor = _CurrentTarget.transform.InverseTransformPoint(_AimPos);

        Joint.spring = 100f;
        Joint.maxDistance = Vector3.Distance(Joint.transform.position, _CurrentTarget.transform.position);
    }

}
