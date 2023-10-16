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

    private int fireRate = 0;


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


        AnimatorClipInfo animationClip = animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex(Utils.Constants.SHOOT))[0];
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(Utils.Constants.SHOOT));
        int currentFrame = (int)(animationState.normalizedTime * (animationClip.clip.length * animationClip.clip.frameRate));



        //Debug.Log("Shoot weight " + shootWeight);

        if (character.IsShooting)
        {
            ShootBullet();


        }
        else
        {
            fireRate = currentFrame;
        }
    }

    private void ShootBullet()
    {
        AnimatorClipInfo animationClip = animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex(Utils.Constants.SHOOT))[0];
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(Utils.Constants.SHOOT));

        int currentFrame = (int)(animationState.normalizedTime * (animationClip.clip.length * animationClip.clip.frameRate));

        if (currentFrame >= fireRate)
        {

            fireRate += (int)(animationClip.clip.length * animationClip.clip.frameRate);

            GameObject cb = Instantiate(laser, spawnPoint.position, spawnPoint.transform.rotation);
            Rigidbody rb = cb.GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        }

    }
}
