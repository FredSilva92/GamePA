using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image HealthBar;
    public float healthAmount = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.A) && healthAmount >= 0)
        {
            Debug.Log("Estou no A");
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Estou no S");
            UpdateHealth(20);
        }*/
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        HealthBar.fillAmount = healthAmount / 100;
    }

    public void UpdateHealth(float healingAmount) {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        HealthBar.fillAmount = healthAmount / 100f;
    }
}
