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

    [SerializeField]
    protected GameObject crossHair;

    protected float time = 0;

    protected CharacterBase character;

    [SerializeField]
    private Camera camera;

    private ThirdPersonCam cameraScript;

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

            if (ThirdPersonCam.CameraStyle.Combat.Equals(cameraScript.CurrentStye))
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2 , 100f);
                Vector3 worldCenter = camera.ScreenToWorldPoint(screenCenter);

                Vector3 forceDirection = worldCenter - transform.position;

                rb.AddForce(forceDirection * 0.5f, ForceMode.Impulse);
            }
            else
            {

                rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * speed, ForceMode.Impulse);
            }
            time = 0;
        }
    }
}