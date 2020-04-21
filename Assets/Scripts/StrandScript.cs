using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrandScript : MonoBehaviour
{

    private float StrandLength; // min distance for the strand
    [SerializeField]
    private GameObject _targetGameObject; // current targeted gameobject
    private Rigidbody _PlayerRB; // player rigidbody if there is one
    private Rigidbody _StrandRB; // This rigidbody
    private Transform _shootingHand;
    
    [SerializeField]
    private GameObject _endObj; // end null object
    
    private Vector3 _startPoint;
    private Vector3 _endPoint;

    [SerializeField]
    private float _springStrength;

    private ConfigurableJoint _confJoint;

    private LineRenderer _strandRenderer;

    private bool _attachedToPlayer = false;
    private bool _TargetHasRB;

    private SpringJoint _strandSpring;

    private void Awake()
    {
        _strandRenderer = GetComponent<LineRenderer>();
        _strandSpring = GetComponent<SpringJoint>();
        _StrandRB = GetComponent<Rigidbody>();
        _confJoint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        UpdateStrandLine();

        if (_attachedToPlayer)
            updateJointAnchor();


    }

    public void initializeStrand(GameObject _target, Vector3 _AimPos, Rigidbody _HandRB, Rigidbody _Player, bool _TargetIsObject) // initialize strand
    {
        

        _PlayerRB = _Player;
        _targetGameObject = _target;
        _shootingHand = _HandRB.transform;
        
        switch (_TargetIsObject) // behaviour depending on target
        {
            case true: // shooting at an object, no link to player

                // base object config
                _confJoint.connectedBody = _HandRB;

                // target config
                _strandSpring.connectedBody = _target.GetComponent<Rigidbody>();
                _strandSpring.autoConfigureConnectedAnchor = false;
                //Joint.connectedAnchor = _AimPos - _CurrentTarget.transform.position;

                _strandSpring.connectedAnchor = _target.transform.InverseTransformPoint(_AimPos);

                //_strandSpring.spring = _springStrength;
                _strandSpring.minDistance = Vector3.Distance(_strandSpring.transform.position, _target.transform.position);

                break;
            case false: // not shooting at object, attach to player
                // base object config
                _confJoint.connectedBody = _Player;
                _attachedToPlayer = true;
                updateJointAnchor();

                // target config
                _endObj.transform.SetParent(_targetGameObject.transform);
                _endObj.GetComponent<Rigidbody>().isKinematic = true;
                break;

        }

        // target point setup
        _endObj.transform.SetParent(null);
        _endObj.transform.position = _AimPos;


       
        
        /*
        Vector3 _worldConnectedAnchorPoint;
        _worldConnectedAnchorPoint = _strandSpring.connectedBody.transform.position + _strandSpring.connectedBody.transform.InverseTransformPoint(_strandSpring.connectedAnchor);

        Debug.DrawLine(_worldConnectedAnchorPoint, _worldConnectedAnchorPoint + Vector3.up * .1f);
        Debug.Break();

        _strandSpring.minDistance = Vector3.Distance(transform.position, _worldConnectedAnchorPoint);*/

    }

    private void initEndObjJoint(ConfigurableJoint Joint)
    {
        JointDrive Drive = new JointDrive();
        Drive.positionSpring = 100000000;
        Drive.positionDamper = 5000;
        Drive.maximumForce = Mathf.Infinity;

        Joint.xDrive = Drive;
        Joint.yDrive = Drive;
        Joint.zDrive = Drive;
    }

    private void UpdateStrandLine()
    {
        _startPoint = transform.position + _strandSpring.anchor;
        _endPoint = _strandSpring.connectedAnchor + _strandSpring.connectedBody.transform.position;


        _strandRenderer.SetPosition(0, _startPoint);
        _strandRenderer.SetPosition(1, _endPoint);
    }

    private void updateJointAnchor()
    {
        Vector3 AnchorOffset;
        //AnchorOffset = _PlayerRB.transform.InverseTransformPoint(_shootingHand.transform.position);
        AnchorOffset = _shootingHand.transform.position - _PlayerRB.transform.position;
        /*
        Debug.DrawLine(_shootingHand.transform.position + transform.up*.05f, _PlayerRB.transform.position + transform.up * .05f, Color.green);
        Debug.DrawLine(_PlayerRB.transform.position + AnchorOffset, _PlayerRB.transform.position, Color.red);
        */

        _strandSpring.anchor = AnchorOffset;
    }

    public void BreakStrand()
    {
        Destroy(_endObj);
        Destroy(gameObject);
    }

}
