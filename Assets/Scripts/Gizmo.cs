using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [SerializeField]
    private Color _GizmoColor;

    [SerializeField]
    private float _Scale;

    private void OnDrawGizmos()
    {
        Gizmos.color = _GizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(_Scale, _Scale, _Scale));
    }
}
