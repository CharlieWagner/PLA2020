using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    private Rigidbody _RB;
    private bool _UnPlugged = true;

    private float speed = 2;
    private Transform _NewTransform;

    private void Start()
    {
        _RB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_UnPlugged && Vector3.Distance(transform.position, _NewTransform.position) >= .01)
            moveToNewTransform();
    }

    public void Plug(Transform _Parent,Transform _newTransform)
    {
        _UnPlugged = false;
        _RB.isKinematic = true;

        _NewTransform = _newTransform;
    }

    private void moveToNewTransform()
    {
        transform.position = Vector3.Lerp(transform.position, _NewTransform.position, Time.deltaTime * speed);
        
        transform.rotation = Quaternion.Lerp(transform.rotation, _NewTransform.rotation, Time.deltaTime * speed * 4);
    }
}
