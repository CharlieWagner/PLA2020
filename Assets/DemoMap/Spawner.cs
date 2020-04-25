using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private BatteryPowered _BP;

    [SerializeField]
    private GameObject _ToSpawn;
    
    void Start()
    {
        _BP = GetComponent<BatteryPowered>();
    }

    private void Update()
    {
        if (_BP._isOn)
        {
            Instantiate(_ToSpawn, transform.position, transform.rotation);
            _BP._isOn = false;
            _BP._Energy = 0;
        }
    }
}
