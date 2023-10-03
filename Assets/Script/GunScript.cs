using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    private float speed = 5f;

    [FormerlySerializedAs("Next fire")]
    [SerializeField]
    private float nextFire;

    private int fireRate = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Animator playerAnim = player.GetComponent<Animator>();
        float shootWeight = playerAnim.GetLayerWeight(playerAnim.GetLayerIndex(Utils.Constants.SHOOT));

        AnimatorClipInfo animationClip = playerAnim.GetCurrentAnimatorClipInfo(playerAnim.GetLayerIndex(Utils.Constants.SHOOT))[0];
        AnimatorStateInfo animationState = playerAnim.GetCurrentAnimatorStateInfo(playerAnim.GetLayerIndex(Utils.Constants.SHOOT));
        int currentFrame = (int)(animationState.normalizedTime * (animationClip.clip.length * animationClip.clip.frameRate));



        Debug.Log("Shoot weight " + shootWeight);

        if (shootWeight > 0.999999f)
        {
            ShootBullet();

            
        } else
        {
            fireRate = currentFrame;
        }
    }

    private void ShootBullet()
    {
        Animator playerAnim = player.GetComponent<Animator>();

        AnimatorClipInfo animationClip = playerAnim.GetCurrentAnimatorClipInfo(playerAnim.GetLayerIndex(Utils.Constants.SHOOT))[0];
        AnimatorStateInfo animationState = playerAnim.GetCurrentAnimatorStateInfo(playerAnim.GetLayerIndex(Utils.Constants.SHOOT));
   
        int currentFrame = (int)(animationState.normalizedTime * (animationClip.clip.length * animationClip.clip.frameRate));

        if (currentFrame >= fireRate) {

            fireRate += (int)(animationClip.clip.length * animationClip.clip.frameRate);

            GameObject cb = Instantiate(laser, spawnPoint.position, laser.transform.rotation);
            Rigidbody rb = cb.GetComponent<Rigidbody>();

            rb.AddForce(spawnPoint.forward * speed, ForceMode.Impulse);
            
        }
        //fireRate++;
        //Debug.Log("Time = " + Time.time);
    }
}
