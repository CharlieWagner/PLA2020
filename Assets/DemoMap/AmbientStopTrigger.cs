using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientStopTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioSource _Ambient;
    private float _volume = 1;
    private bool _startDecrease = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_startDecrease && (other.tag == "Player"))
            _startDecrease = true;
    }

    private void Update()
    {
        if (_startDecrease)
        {
            _volume -= Time.deltaTime * .33f;
            _Ambient.volume = _volume;
        }
    }
}
