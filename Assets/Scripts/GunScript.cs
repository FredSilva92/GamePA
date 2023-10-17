using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class GunScript : MonoBehaviour
{
    [FormerlySerializedAs("Spawn Point")]
    [SerializeField]
    private Transform spawnPoint;
    
    [SerializeField]
    private GameObject laser;
    
    [SerializeField] 
    private GameObject player;

    [SerializeField]
    private Animator animator;
    
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float fireRate = 0;

    private float time = 0;


    private CharacterBase character;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        character = player.GetComponentInParent<CharacterBase>();

        if (character.IsShooting)
        {
            ShootBullet();  
        }


        //Debug.Log("Shoot weight " + shootWeight);

        //bool checkShoot = Time.time > lastShootTime + fireRate;


        if (character.IsShooting)
        {
            ShootBullet();

        }
    }

    private void ShootBullet()
    {
        time += Time.deltaTime;
        float nextTimeToFire = 1 / fireRate;

        if (time >= nextTimeToFire)
        {
            GameObject cb = Instantiate(laser, spawnPoint.position, spawnPoint.transform.rotation);
            Rigidbody rb = cb.GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
            time = 0;
        }
    }
}
