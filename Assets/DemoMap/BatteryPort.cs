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


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide");
        if (_CurrentBattery == null)
        {
            Debug.Log("can be plugged");
            if (collision.gameObject.TryGetComponent(out _CurrentBattery))
            {
                Debug.Log("plugging");
                _CurrentBattery.Plug(transform, _BatteryPosition);
                _Powering._Energy += 1;
            }
        }
    }

}
