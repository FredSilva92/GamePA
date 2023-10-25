using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerGunScript : MonoBehaviour
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
    protected float speed = 5f;

    [SerializeField]
    protected float fireRate = 0;

    protected float time = 0;


    protected CharacterBase character;

    [SerializeField]
    private Camera camera;

    private ThirdPersonCam cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        cameraScript = camera.GetComponent<ThirdPersonCam>();
    }

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

            Debug.Log("Gun: " + transform.forward);

            if (ThirdPersonCam.CameraStyle.Combat.Equals(cameraScript.CurrentStye))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Camera.main.transform.position.z; // Set the z coordinate to match the camera's

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector3 forceDirection = mouseWorldPos - transform.position;

                rb.AddForce(forceDirection * 0.2f, ForceMode.Impulse);
            } else
            {
                rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * speed, ForceMode.Impulse);
            }
            time = 0;
        }
    }
}
