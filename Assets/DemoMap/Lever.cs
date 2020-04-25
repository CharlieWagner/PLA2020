using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private Transform _Handle;

    [Range(0,360)]
    [SerializeField]
    private float _MinAngle;
    [Range(0, 360)]
    [SerializeField]
    private float _MaxAngle;
    [SerializeField]
    private float _NewTargetAngle;

    private bool _pulled = false;
    private HingeJoint _Joint;

    [SerializeField]
    private BatteryPowered _Powering;

    private AudioSource _AudioSource;

    //target position

    void Start()
    {
        _AudioSource = GetComponent<AudioSource>();
        _Joint = GetComponent<HingeJoint>();
        _Handle = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!_pulled)
        {

            if (_Handle.localEulerAngles.x >= _MinAngle && _Handle.localEulerAngles.x <= _MaxAngle)
            {
                _Powering._Energy += 1;
                _pulled = true;
                
                JointSpring hingeSpring = _Joint.spring;
                hingeSpring.targetPosition = _NewTargetAngle;
                hingeSpring.spring = 1000f;
                hingeSpring.damper = 50f;

                _Joint.spring = hingeSpring;
                _AudioSource.Play();
            }
        }

    }
}
