using UnityEngine;

public class Utils
{
    public static class Constants
    {
        public static readonly string AIM_KEY = "Fire2";
        public static readonly string SHOOT_KEY = "Fire1";
        public static readonly string SHOOT = "ShootAnim";
        public static readonly string LAZER_BULLET_PLAYER = "LazerBulletPlayer";
        public static readonly string LAZER_BULLET_ENEMY = "LazerBulletEnemy";
    }

    public static void DeathAnimation(Animator animator)
    {
        animator.SetBool("isWalking", false);
        animator.SetLayerWeight(animator.GetLayerIndex(Constants.SHOOT), 0f);
        animator.SetBool("isDying", true);
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
}