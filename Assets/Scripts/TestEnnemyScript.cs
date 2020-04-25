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

    private float _Awareness = .2f;
    private AudioSource _AudioSource;

    private int _HP = 1;
    private Rigidbody _RB;

    private void Start()
    {
        _Drive = GetComponent<ConfigurableJoint>();
        _AudioSource = GetComponent<AudioSource>();
        _Drive.connectedAnchor = transform.position;
        _TargetPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _Player.transform.position) <= 5 && !_activated)
        {
            _Animator.SetTrigger("Startup");
            _AudioSource.Play();
            _TargetPosition = transform.position + new Vector3(0,2,0);
            _activated = true;
        }
        if (_activated)
        {
            _Drive.connectedAnchor = _TargetPosition;
            _Drive.targetRotation = Quaternion.Inverse(_TargetRotation);//_TargetRotation;

            if (Vector3.Distance(_TargetPosition, transform.position) <= 1f)
                _TargetPosition = Vector3.MoveTowards(_TargetPosition, _LastKnownPos + new Vector3(0, 2, 0), _speed * Time.deltaTime);



            RaycastHit PlayerCheck;

            Vector3 PlayerDir = _Player.transform.position - transform.position;
            Vector3 DebugTargetDir = _LastKnownPos - transform.position;

            Debug.DrawRay(transform.position + ((PlayerDir).normalized * .5f), PlayerDir - ((PlayerDir).normalized * .5f), Color.red);
            Debug.DrawRay(transform.position + ((DebugTargetDir).normalized * .5f) + new Vector3(0, .1f, 0), DebugTargetDir - ((DebugTargetDir).normalized * .5f), Color.red);
            if (!Physics.SphereCast(transform.position/* + ((PlayerDir).normalized * .5f)*/, .45f, PlayerDir, out PlayerCheck, PlayerDir.magnitude - .5f, _NotPlayer))
            {
                _LastKnownPos = _Player.transform.position;
                _Awareness += Time.deltaTime * .2f;
            } else
            {
                _Awareness -= Time.deltaTime * .1f;
            }

            _Awareness = Mathf.Clamp(_Awareness, .2f, 1.1f);

            _AudioSource.pitch = _Awareness;


        } else
        {
            _Awareness -= Time.deltaTime * .3f;
            _Awareness = Mathf.Clamp(_Awareness, .2f, 1.1f);
        }

        if (_HP <= 0 && _HP > -5)
        {
            Kill();
        }

        

        Vector3 relativePos = _Player.transform.position - transform.position;
        relativePos = _LastKnownPos - transform.position;
        _TargetRotation = Quaternion.LookRotation(relativePos , Vector3.up);
        //transform.rotation = _TargetRotation;
    }

    public void Damage(Vector3 position)
    {
        _HP--;
        _RB.AddForceAtPosition(Vector3.up * 10, position);
    }

    private void Kill()
    {
        Destroy(_Drive);
        _AudioSource.loop = false;
        _HP = -5;
    }
}
