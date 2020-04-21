using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrandScript : MonoBehaviour
{

    private float StrandLength; // min distance for the strand
    private GameObject _targetGameObject; // current targeted gameobject
    private Rigidbody _PlayerRB; // player rigidbody if there is one
    private Rigidbody _StrandRB; // This rigidbody
    private Transform _shootingHand;
    
    [SerializeField]
    private GameObject _endObj; // end null object
    
    private Vector3 _startPoint;
    private Vector3 _endPoint;

    private LineRenderer _strandRenderer;

    private bool _TargetHasRB;

    private SpringJoint _strandSpring;

    private void Start()
    {
        _strandRenderer = GetComponent<LineRenderer>();
        _strandSpring = GetComponent<SpringJoint>();
        _StrandRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateStrandLine();
    }

    public void initializeStrand(GameObject _target, Vector3 _AimPos, Rigidbody _HandRB, Rigidbody _Player, bool _TargetIsObject) // initialize strand
    {
        _PlayerRB = _Player;
        _targetGameObject = _target;
        _shootingHand = _HandRB.transform;


        // shoot point setup



        ConfigurableJoint _confJoint = GetComponent<ConfigurableJoint>();
        
        switch (_TargetIsObject) // behaviour depending on target
        {
            case true: // shooting at an object, no link to player
                //transform.SetParent(_Hand.transform);
                _confJoint.connectedBody = _HandRB;
                //_StrandRB.isKinematic = true;
                break;
            case false: // not shooting at object, attach to player
                _confJoint.connectedBody = _HandRB;
                break;

        }

        // target point setup
        _endObj.transform.SetParent(null);
        _endObj.transform.position = _AimPos;
        

        _TargetHasRB = _targetGameObject.TryGetComponent(out Rigidbody _targetRB);

        
        

        switch (_TargetHasRB) // behaviour depending on target
        {
            case true: // link the endObject to the target with a fixed joint
                
                ConfigurableJoint _endObjConfJoint = _endObj.AddComponent<ConfigurableJoint>();
                initEndObjJoint(_endObjConfJoint);
                _endObjConfJoint.connectedBody = _targetRB;
                //_endObjConfJoint.autoConfigureConnectedAnchor = true;
                
                //_endObj.GetComponent<Rigidbody>().isKinematic = false;
                
                break;
            case false: // set endObject as child of target
                _endObj.transform.SetParent(_targetGameObject.transform);
                _endObj.GetComponent<Rigidbody>().isKinematic = true;
                break;

        }
        
        Vector3 _worldConnectedAnchorPoint;
        _worldConnectedAnchorPoint = _strandSpring.connectedBody.transform.position + _strandSpring.connectedBody.transform.InverseTransformPoint(_strandSpring.connectedAnchor);

        Debug.DrawLine(_worldConnectedAnchorPoint, _worldConnectedAnchorPoint + Vector3.up * .1f);
        Debug.Break();

        _strandSpring.minDistance = Vector3.Distance(transform.position, _worldConnectedAnchorPoint);

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
        _endPoint = _endObj.transform.position;


        _strandRenderer.SetPosition(0, _startPoint);
        _strandRenderer.SetPosition(1, _endPoint);
    }


    public void BreakStrand()
    {
        Destroy(_endObj);
        Destroy(gameObject);
    }

}
