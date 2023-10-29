using UnityEngine;

public class Utils
{
    public static class Constants
    {
        public static readonly string AIM_KEY = "Fire2";
        public static readonly string SHOOT_KEY = "Fire1";
        public static readonly string SHOOT = "ShootAnim";
        public static readonly string PICK = "Pick";
        public static readonly string LAZER_BULLET_PLAYER = "LazerBulletPlayer";
        public static readonly string LAZER_BULLET_ENEMY = "LazerBulletEnemy";
        public static readonly string MEDICINE = "Medicine";
    }

    public static class Animations
    {
        public static readonly string WALKING = "isWalking";
        public static readonly string DYING = "isDying";
        public static readonly string PICKING = "isPicking";
        public static readonly string SHOOTING = "isShooting";
        public static readonly string JUMPING = "isJumping";

    }

    public static void PlayAnimation(Animator animator, string animation)
    {
        animator.SetBool(Animations.WALKING, false);
        animator.SetLayerWeight(animator.GetLayerIndex(Constants.SHOOT), 0f);
        animator.SetBool(animation, true);
    }

    public static void CheckIfWasHitShooted(Collider collision, HealthManager healthManager, string bulletRef, ref bool isDead)
    {
        if (collision.gameObject.CompareTag(bulletRef))
        {
            LaserBulletScript laserScript = collision.GetComponent<LaserBulletScript>();
            healthManager.TakeDamage(laserScript.Damage);
            isDead = healthManager.Health <= 0;
        };
    }

    public static float GetDistanceBetween2Objects(GameObject object1, GameObject object2)
    {
        return Vector3.Distance(object1.transform.position, object2.transform.position);
    }
}