using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private BatteryPowered _BP;
    private Animator _Anim;

    private AudioSource _AudioSource;

    private void Start()
    {
        _Anim = GetComponent<Animator>();
        _BP = GetComponent<BatteryPowered>();
        _AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_BP._isOn)
        {
            _Anim.SetTrigger("Open");
        }
    }

    public void PlaySound()
    {
        _AudioSource.Play();
    }
}
