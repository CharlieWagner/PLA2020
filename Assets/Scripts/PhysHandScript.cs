using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysHandScript : MonoBehaviour
{

    [SerializeField]
    private Rigidbody _PlayerRB;

    [SerializeField]
    private HandScript _ParentHand;// Controller
    [SerializeField]
    private LayerMask GrabMask;    // Grabbing mask, excludes player
    [SerializeField]
    private int side = 1;          // the side of the hand

    private FixedJoint _GrabJoint; // the fixed joint used for grabbing stuff

    private Rigidbody RB;

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
    private GameObject _GrapplePointPrefab;
    private SpringJoint _GrappleSpring;

    [SerializeField]
    private float _ObjectSpringStrength;

    private bool _Grabbing; // Is player grabbing something/has it's fist closed
    private bool _Shooting; // Is the player shooting
    private int _GrabType; // Type of grab

    private bool _HasPulled;
    [SerializeField]
    private float _MinPullForce;

    [SerializeField]
    private LineRenderer _ShootLine; // Shooting Line Renderer
    [SerializeField]
    private Transform _VRRig;
    

    private void Start()
    {
        //_GrabJoint = GetComponent<FixedJoint>();
        RB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_Shooting && !_Grabbing)
            TargetStatus();
        else if (_Grabbing)
            _RangeStatus = 0;

        LineUpdate();
        ReticleUpdate();
        updateShootLine();
        HandAnimControl();
        

        Shoot();

        Grab();
    }

    private void Grab()
    {
        if (_ParentHand._Grab.GetStateDown(_ParentHand._handType) && !_Shooting)
        {
            RaycastHit Grab;

            //if (Physics.SphereCast(transform.position, .1f, side * transform.right, out Grab, .02f, GrabMask))

            //if (Physics.Raycast(transform.position, side * transform.right, out Grab, .1f, GrabMask))
            if (Physics.SphereCast(transform.position - (side * transform.right)*.05f, .05f, side * transform.right, out Grab, .05f, GrabMask))
            {
                if (_GrabJoint == null)
                {
                    _GrabJoint = gameObject.AddComponent<FixedJoint>();
                }
                _GrabJoint.connectedBody = Grab.rigidbody;
                _GrabType = 1;
            }
            else
            {
                _GrabType = 0;
            }
            _Grabbing = true;


            Debug.DrawRay(transform.position, side * transform.right, Color.red);
        }

        if (_ParentHand._Grab.GetStateUp(_ParentHand._handType))
        {
            _Grabbing = false;
            Destroy(_GrabJoint);
        }
    } // Grab Manager

    private void TargetStatus()
    {
        RaycastHit _TargetInfo;
        //if (Physics.Raycast(_ShootSpot.position, _ShootSpot.forward, out _TargetInfo, _Range, _DynamicObjectLayerMask))
        if (Physics.Raycast(_ShootSpot.position, _ShootSpot.forward, out _TargetInfo, 1, _DynamicObjectLayerMask)
            && !Physics.Raycast(_ShootSpot.position, _TargetInfo.point - _ShootSpot.position, Vector3.Distance(_ShootSpot.position, _TargetInfo.point), _NotDynamicObject))
        {
            _RangeStatus = 1;
            _TargetIsObject = true;

            _CurrentTarget = _TargetInfo.collider.gameObject;

        }
        else if (Physics.SphereCast(_ShootSpot.position + (_ShootSpot.forward * .5f), _ObjectAimSpread, _ShootSpot.forward, out _TargetInfo, _Range, _DynamicObjectLayerMask)
            && !Physics.Raycast(_ShootSpot.position, _TargetInfo.point - _ShootSpot.position, Vector3.Distance(_ShootSpot.position, _TargetInfo.point), _NotDynamicObject))
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
        else if (Physics.SphereCast(_ShootSpot.position + (_ShootSpot.forward * .5f), _ObjectAimSpread, _ShootSpot.forward, out _TargetInfo, _Range, _NotDynamicObject))
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
    } // Targeting control

    private void LineUpdate()
    {
        if (_RangeStatus == 1)
            _AimLine.SetPosition(1, _AimPos);
        else
            _AimLine.SetPosition(1, _AimLine.transform.position);

        _AimLine.SetPosition(0, _AimLine.transform.position);
    } // Targeting line update

    private void ReticleUpdate() // Reticle animations manager
    {
        _ReticleAnimator.SetInteger("RangeStatus", _RangeStatus);
        _ReticleAnimator.SetBool("isObject", _TargetIsObject);
    }

    private void HandAnimControl()
    {
        _HandAnimator.SetBool("Grabbing", _Grabbing);
        _HandAnimator.SetInteger("GrabType", _GrabType);
        _HandAnimator.SetBool("Shooting", _Shooting);
    }

    private void Shoot() // Shoot input & logic
    {
        if (_ParentHand._Shoot.GetStateDown(_ParentHand._handType) && _RangeStatus == 1 && !_Grabbing) // Shoot press
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
                    _CurrentTarget = Instantiate(_GrapplePointPrefab, _AimPos, Quaternion.identity);
                    _GrappleSpring = _CurrentTarget.GetComponent<SpringJoint>();
                    initPlayerShootJoint();



                    _RangeStatus = 2;
                    break;
            }
            _Shooting = true;
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

                    Destroy(_CurrentTarget);

                    _CurrentTarget = null;
                    _RangeStatus = 0;
                    break;
            }
            _Shooting = false;
            _HasPulled = false;
        }


        if (_Shooting) // ---------------------------------- IF THE PLAYER IS SHOOTING
        {
            switch (_TargetIsObject)
            {
                case true: //   Target is object
                    Pull();
                    break;
                case false: //  Target is not object
                    Pull();
                    updatePlayerShootJoint();
                    break;
            }
        }
    }

    private void updateShootLine() // Update grapple Line renderer positions
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
                    _ShootLine.SetPosition(1, _GrappleSpring.transform.position); 
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

        Joint.spring = _ObjectSpringStrength;
        Joint.maxDistance = Vector3.Distance(Joint.transform.position, _CurrentTarget.transform.position);
    }

    private void initPlayerShootJoint()
    {
        _GrappleSpring.connectedBody = _PlayerRB;
        Vector3 AnchorOffset = Vector3.zero;
        //Debug.Break();
        //Vector3 AnchorOffset = _PlayerRB.transform.InverseTransformPoint(transform.localPosition);
        _GrappleSpring.connectedAnchor = AnchorOffset;
        _GrappleSpring.maxDistance = Vector3.Distance(_GrappleSpring.transform.position, /*_PlayerRB.*/transform.position);
    }

    private void updatePlayerShootJoint()
    {
        Vector3 AnchorOffset = Vector3.zero;
        //Vector3 AnchorOffset = _PlayerRB.transform.InverseTransformPoint(transform.localPosition);
        Debug.DrawLine(_PlayerRB.transform.position, _PlayerRB.transform.position + AnchorOffset);
        _GrappleSpring.connectedAnchor = AnchorOffset;
        _GrappleSpring.connectedAnchor = Vector3.zero + _PlayerRB.transform.TransformPoint(transform.localPosition) - _PlayerRB.transform.position;
    }

    private void Pull()
    {
        if (RB.velocity.magnitude > _MinPullForce && !_HasPulled)
        {
            if (Vector3.Dot(RB.velocity.normalized, (_CurrentTarget.transform.position - transform.position).normalized) < -.7f)
            {
                _ObjectShootJoint.maxDistance = _ObjectShootJoint.maxDistance - 1;
                _ObjectShootJoint.connectedBody.velocity += Vector3.up;

                _HasPulled = true;
            }
        }
    }

}
