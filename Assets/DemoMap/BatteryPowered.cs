using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPowered : MonoBehaviour
{
    public int _Energy;
    [SerializeField]
    private int _MinEnergy;

    public bool _isOn = false;


    private void Update()
    {
        if (_Energy >= _MinEnergy)
            _isOn = true;
    }
}
