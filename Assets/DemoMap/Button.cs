using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField]
    private Transform _Button;

    private bool _Clicked;

    [SerializeField]
    private BatteryPowered _Powering;

    private AudioSource _AudioSource;

    private void Start()
    {
        _AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_Button.localPosition.y <= .75f && !_Clicked)
        {
            Click();
            _Clicked = true;
        } else if (_Button.localPosition.y >= .77f)
        {
            _Clicked = false;
        }
    }

    private void Click()
    {
        _Powering._Energy += 1;

        _AudioSource.Play();
    }
}
