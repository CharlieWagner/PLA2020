using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Contrôles déplacement")]
    [SerializeField]
    private float _GroundSpeed;
    [SerializeField]
    private float _MaxAcceleration;
    [SerializeField]
    private float _JumpAcceleration;
    [SerializeField]
    private float _AirControl;

    private float _distToGround;
    private float _distToCapsuleCenter;
    private float _distToSide;
    public float _MaxVelocity;
    private Vector3 _LastVelocity;
    private Vector3 _CurrentAcceleration;
    [SerializeField]
    private GameObject _FovCone;
    private Material _FovConeMat;
    public Rigidbody _PlayerRB;
    [SerializeField]
    private Transform _SteamVRRig;
    [SerializeField]
    private Transform TestViewHead;
    private Vector3 _lastHeadPos;
    private Vector3 _headVelocity;
    [SerializeField]
    private float _MoveDeadzone;
    [Header("VR Input")]
    [SerializeField]
    private SteamVR_Input_Sources _handType;
    [SerializeField]
    private SteamVR_Action_Vector2 _Move;
    [SerializeField]
    private SteamVR_Action_Boolean _Jump;
    [SerializeField]
    private SteamVR_Action_Boolean _SnapTurnLeft;
    [SerializeField]
    private SteamVR_Action_Boolean _SnapTurnRight;
    [SerializeField]
    private SteamVR_Action_Boolean _Crouch;
    [SerializeField]
    private GameObject _VRCam;
    [SerializeField]
    private Transform _HeadPosition;


    [Header("Debug UI")]

    [SerializeField]
    private Transform _Stick;

    private void Start()
    {
        _distToGround = GetComponent<Collider>().bounds.extents.y;
        _distToCapsuleCenter = GetComponent<Collider>().bounds.center.y;
        _distToSide = GetComponent<Collider>().bounds.extents.x;
        _FovConeMat = _FovCone.GetComponent<Renderer>().material;
    }

    private void FixedUpdate()
    {
        VRRigRecenter();

        UpdateStickUI();

        if (isGrounded())
        {
            AccelerateTowards(BaseVelocityTarget(_GroundSpeed) + _headVelocity);

            
            if (_Jump.stateDown)
            {
                _PlayerRB.AddForce(new Vector3(0, _JumpAcceleration, 0));
            }
        }
        else
        {
            /*float dotVectors;
            dotVectors = Vector3.Dot(BaseVelocityTarget(_AirControl).normalized, new Vector3(_PlayerRB.velocity.x, 0, _PlayerRB.velocity.z).normalized);
            dotVectors = -dotVectors + 1;
            dotVectors = Mathf.Clamp(dotVectors, 0, 1);
            _PlayerRB.AddForce(BaseVelocityTarget(_AirControl) * dotVectors);*/
        }

        FovReduce(10,.5f, .5f);
        //ClampVelocity();
        SnapTurn(45);
        
    }

    private Vector3 BaseVelocityTarget(float Speed) // Base X & Z axis velocity target
    {
        float yRotation = _VRCam.transform.eulerAngles.y;
        Quaternion Forwards = Quaternion.Euler(0,yRotation,0);

        if (_Move.axis.magnitude >= _MoveDeadzone)
            return (Forwards * new Vector3(_Move.axis.x, 0, _Move.axis.y) * Speed);
        else 
            return (Vector3.zero);
    }

    private void AccelerateTowards(Vector3 Target)
    {
        Vector3 Acceleration;
        Vector3 XZVelocity;
        XZVelocity = _PlayerRB.velocity - new Vector3(0, _PlayerRB.velocity.y, 0);

        //XZVelocity -= _headVelocity;
        
        Acceleration = Target - XZVelocity;
        _PlayerRB.AddForce(Acceleration.normalized * Mathf.Clamp(Acceleration.magnitude, 0, _MaxAcceleration));

        //Debug.Log(Acceleration.magnitude);
    }

    private bool isGrounded()
    {
        RaycastHit Hit;
        return Physics.SphereCast(transform.position /*- new Vector3(0,_distToCapsuleCenter,0)*/, _distToSide, -Vector3.up, out Hit, _distToGround + 0.01f);
    }

    private void SnapTurn(float Angle)
    {
        if (_SnapTurnLeft.stateDown)
        {
            transform.Rotate(new Vector3(0,-Angle,0));
        } else if (_SnapTurnRight.stateDown)
        {
            transform.Rotate(new Vector3(0, Angle, 0));
        }
    }

    private void VRRigRecenter()
    {
        _SteamVRRig.transform.localPosition = new Vector3(-_VRCam.transform.localPosition.x, _SteamVRRig.transform.localPosition.y, -_VRCam.transform.localPosition.z);

        _headVelocity = ((_VRCam.transform.localPosition + _HeadPosition.localPosition) - _lastHeadPos) / Time.deltaTime;
        _lastHeadPos = (_VRCam.transform.localPosition + _HeadPosition.localPosition);
        
        _headVelocity = new Vector3(_headVelocity.x, 0, _headVelocity.z);

        _headVelocity = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * _headVelocity;

        //Debug.DrawLine(transform.position + Vector3.up*.05f, transform.position + _headVelocity + Vector3.up * .05f);


        //Debug.DrawLine(transform.position, transform.position + _headVelocity*60);
        //Debug.Log(_headVelocity.magnitude);
    }

    private void ClampVelocity()
    {
        _PlayerRB.velocity = _PlayerRB.velocity.normalized * Mathf.Clamp(_PlayerRB.velocity.magnitude, 0, _MaxVelocity);
    }

    private void FovReduce(float strength, float maxFovPercent, float lerpSpeed)
    {
        float targetValue;

        _CurrentAcceleration = _LastVelocity - _PlayerRB.velocity;
        _LastVelocity = _PlayerRB.velocity;
        targetValue = Mathf.Clamp(_CurrentAcceleration.magnitude * strength, 0, maxFovPercent);
        _FovConeMat.SetFloat("_Ctrl", Mathf.Lerp(_FovConeMat.GetFloat("_Ctrl"), targetValue, Time.deltaTime * lerpSpeed));
    }


    private void UpdateStickUI()
    {
        _Stick.transform.localPosition = new Vector3(_Move.axis.x * 30, _Move.axis.y * 30, -.5f);
    }
}
