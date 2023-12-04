using UnityEngine;

public class LaserBulletScript : MonoBehaviour
{
    [SerializeField]
    private float _damage = 20f;

    [SerializeField]
    private GameObject muzzlePrefab;

    [SerializeField]
    private GameObject hitPrefab;

    public float Damage { get { return _damage; } }

    private void Start()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, transform.rotation);
            muzzleVFX.transform.forward = transform.forward;
        }
        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        RaycastHit hit;
        if (hitPrefab != null && Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Instantiate(hitPrefab, transform.position, hit.transform.rotation);
        }

        Destroy(gameObject);
    }
}