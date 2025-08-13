using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthShield : MonoBehaviour
{
    public int maxHealth = 100;
    public int maxShield = 100;

    public int currentHealth;
    public int currentShield;

    public Image healthBarFill;
    public Image shieldBarFill;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;

        UpdateUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.J)) Heal(10);
        if (Input.GetKeyDown(KeyCode.K)) AddShield(10);

    }

    public void TakeDamage(int amount)
    {
        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(amount, currentShield);
            currentShield -= shieldDamage;
            amount -= shieldDamage;
        }

        if (amount > 0)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        UpdateUI();
        Debug.Log("TakeDamage called with: " + amount); // Add this
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    public void AddShield(int amount)
    {
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        UpdateUI();
    }

    void UpdateUI()
    {
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        shieldBarFill.fillAmount = (float)currentShield / maxShield;
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // You can trigger game over here
    }
}
