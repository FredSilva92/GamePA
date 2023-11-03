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
                RectTransform crossHairRect = crossHair.GetComponent<RectTransform>();

                Vector3 crossHairCenter = GetCrossHairWorldCoordinates(crossHairRect);
                Vector3 forceDirection = crossHairCenter - transform.position;

                rb.AddForce(forceDirection * 0.05f, ForceMode.Impulse);
            }
            else
            {

                rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * speed, ForceMode.Impulse);
            }
            time = 0;
        }
    }

    private Vector3 GetCrossHairWorldCoordinates(RectTransform crossHairRect)
    {
        Vector3[] screenCenter = new Vector3[4];
        crossHairRect.GetWorldCorners(screenCenter);

        float x = screenCenter[2].x + 20f;
        float y = screenCenter[1].y + 10f;

        return camera.ScreenToWorldPoint(new Vector3(x, y, camera.transform.position.z));
    }
}