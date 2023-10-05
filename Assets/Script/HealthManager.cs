using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private Image HealthBar;

    [SerializeField]
    private float initialHealth = 100f;

    private float _currentHealth;

    public float Health { get { return _currentHealth;} }


    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = initialHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        UpdateBar();
    }

    public void UpdateHealth(float healingAmount) {
        _currentHealth += healingAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, initialHealth);

        UpdateBar();
    }

    public void UpdateBar() { 
        if (HealthBar == null)
        {
            return;
        }

        HealthBar.fillAmount = _currentHealth / initialHealth;
    }
}
