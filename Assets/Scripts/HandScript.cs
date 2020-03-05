using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _LinkedHand;
    public SteamVR_Input_Sources _handType; // 1
    public SteamVR_Action_Boolean _Shoot; // 2
    public SteamVR_Action_Boolean _Grab; // 3

    public Transform _Player;
    public Transform _HandTargetPos;
    public Transform _HandAttachPoint;
    public ConfigurableJoint _HandAttachJoint;


    private void Update()
    {
        //_HandAttachPoint.rotation = Quaternion.identity;
        //_HandAttachJoint.targetPosition = _HandTargetPos.position - _Player.position;
        //_HandAttachJoint.targetRotation = _HandTargetPos.rotation;

        //_LinkedHand.rotation = transform.rotation;
    }
}
