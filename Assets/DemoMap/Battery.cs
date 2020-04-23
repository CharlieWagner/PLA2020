using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    private Rigidbody _RB;
    private bool _UnPlugged = true;

    private void Start()
    {
        _RB = GetComponent<Rigidbody>();
    }

    public void Plug(Transform _Parent,Transform _newTransform)
    {
        _UnPlugged = false;
        _RB.isKinematic = true;

        //transform.parent = _Parent;
        transform.position = _newTransform.position;
        transform.rotation = _newTransform.rotation;

    }
}
