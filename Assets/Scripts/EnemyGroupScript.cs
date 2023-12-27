using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utils;

public class EnemyGroupScript : MonoBehaviour
{
    private List<GameObject> enemies;

    
    bool detectionActivated = true;
    // Start is called before the first frame update
    void Start()
    {
        enemies = transform.Cast<Transform>().Select(t => t.gameObject).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        bool isPlayerDetected = false;
        if (!detectionActivated) return;

        foreach (GameObject child in enemies)
        {
            // Access each child GameObject here
            EnemyScript enemyScript = child.GetComponent<EnemyScript>();

            //if (detectionActivated){
                isPlayerDetected = EnemyStates.ATTACK.Equals(enemyScript.CurrentStateName) || EnemyStates.CHASE.Equals(enemyScript.CurrentStateName);

                Debug.Log("Player detected: " + isPlayerDetected);
                if (isPlayerDetected)
                {
                    break;
                }
            /*} else
            {
                detectionActivated = EnemyStates.IDLE.Equals(enemyScript.CurrentStateName) || EnemyStates.PATROL.Equals(enemyScript.CurrentStateName);

                if (detectionActivated) {
                    break;
                }
            }*/
        }

        

        if (!isPlayerDetected) return;

        foreach (GameObject child in enemies)
        {
            // Access each child GameObject here
            EnemyScript enemyScript = child.GetComponent<EnemyScript>();

            enemyScript.SetGroupState(Utils.EnemyStates.CHASE);
        }

        detectionActivated = false;
        ReactivateDetection();
    }

    private IEnumerator ReactivateDetection()
    {
        yield return new WaitForSeconds(30f);
        detectionActivated = true;
    }
}
