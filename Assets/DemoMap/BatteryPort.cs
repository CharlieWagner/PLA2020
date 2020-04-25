using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPort : MonoBehaviour
{
    [SerializeField]
    private Transform _BatteryPosition;

    private Battery _CurrentBattery;

    [SerializeField]
    private BatteryPowered _Powering;


    private AudioSource _AudioSource;

    private void Start()
    {
        _AudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_CurrentBattery == null)
        {
            if (collision.gameObject.TryGetComponent(out _CurrentBattery))
            {
                _CurrentBattery.Plug(transform, _BatteryPosition);
                _Powering._Energy += 1;

                _AudioSource.Play();
            }
        }
    }

}
