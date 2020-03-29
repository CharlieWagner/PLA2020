using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnnemyScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _Player;
    [SerializeField]
    private Animator _Animator;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private LayerMask _NotPlayer;

    private ConfigurableJoint _Drive;
    private bool _activated = false;
    private Vector3 _TargetPosition;
    private Quaternion _TargetRotation;

    private Vector3 _LastKnownPos;


    private void Start()
    {
        _Drive = GetComponent<ConfigurableJoint>();

        _Drive.connectedAnchor = transform.position;
        _TargetPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _Player.transform.position) <= 5 && !_activated)
        {
            _Animator.SetTrigger("Startup");
            _TargetPosition = transform.position + new Vector3(0,2,0);
            _activated = true;
        }
        if (_activated)
        {
            _Drive.connectedAnchor = _TargetPosition;
            _Drive.targetRotation = Quaternion.Inverse(_TargetRotation);//_TargetRotation;

            _TargetPosition = Vector3.MoveTowards(_TargetPosition, _LastKnownPos + new Vector3(0, 2, 0), _speed * Time.deltaTime);
            _LastKnownPos = _Player.transform.position;
            /*RaycastHit PlayerCheck;
            if (!Physics.Raycast(transform.position, _Player.transform.position - transform.position, out PlayerCheck, 25f, _NotPlayer))
            {
                
            }*/
        }

        

        Vector3 relativePos = _Player.transform.position - transform.position;
        _TargetRotation = Quaternion.LookRotation(relativePos , Vector3.up);
        //transform.rotation = _TargetRotation;
    }
}
