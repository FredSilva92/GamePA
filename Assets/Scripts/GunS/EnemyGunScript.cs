using UnityEngine;
using UnityEngine.Serialization;

public class EnemyGunScript : MonoBehaviour
{
    [FormerlySerializedAs("Spawn Point")]
    [SerializeField]
    protected Transform spawnPoint;

    [SerializeField]
    protected GameObject laser;

    [SerializeField]
    protected GameObject player;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected float speed = 10f;

    [SerializeField]
    protected float fireRate = 0;

    protected float time = 0;

    protected CharacterBase character;
  
    void Update()
    {
        character = player.GetComponentInParent<CharacterBase>();

        if (character.IsShooting)
        {
            ShootBullet();
        }
    }

    protected void ShootBullet()
    {
        time += Time.deltaTime;
        float nextTimeToFire = 1 / fireRate;

        if (time >= nextTimeToFire)
        {
            GameObject cb = Instantiate(laser, spawnPoint.position, spawnPoint.transform.rotation);
            Rigidbody rb = cb.GetComponent<Rigidbody>();

            rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * speed, ForceMode.Impulse);
            time = 0;
        }
    }
}