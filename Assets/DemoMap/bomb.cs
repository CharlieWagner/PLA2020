using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    [SerializeField]
    private float _MinSpeed;
    [SerializeField]
    private GameObject _Explosion;

    private bool _blown = false;

    private Rigidbody _RB;
    private AudioSource _AS;

    private void Start()
    {
        _RB = GetComponent<Rigidbody>();
        _AS = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "EnnemyCube")
        {
            Instantiate(_Explosion, transform.position, Quaternion.identity);
            transform.localScale = new Vector3(.001f, .001f, .001f);
            _blown = true;
            _AS.Play();
            other.GetComponent<TestEnnemyScript>().Damage(transform.position);
        }
    }
    
}
