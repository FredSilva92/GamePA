using UnityEngine;

public class LaserBulletScript : MonoBehaviour
{
    [SerializeField]
    private float _damage = 20f;

    public float Damage { get { return _damage; } }

    private void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}