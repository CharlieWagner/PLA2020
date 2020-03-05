using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScript : MonoBehaviour
{
    [Header("Position")]
    [SerializeField]
    private Transform _VRHead;

    [SerializeField]
    private float _positionSpeed;
    [SerializeField]
    private float _rotationSpeed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _VRHead.position, _positionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _VRHead.rotation, _rotationSpeed * Time.deltaTime);
    }
}
