using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBulletScript : MonoBehaviour
{
    [SerializeField]
    private float _damage = 20f;

    public float Damage { get { return _damage; } }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
