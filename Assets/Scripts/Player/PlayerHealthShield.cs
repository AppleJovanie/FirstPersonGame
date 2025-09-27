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

    // --- THIS IS THE MODIFIED METHOD ---
    public void TakeDamage(int amount)
    {
        Debug.Log($"--- Player taking {amount} incoming damage. [Current Shield: {currentShield}, Current Health: {currentHealth}] ---");

        int damageToShield = 0;
        int damageToHealth = 0;

        // First, apply damage to the shield if it exists.
        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(amount, currentShield);
            currentShield -= shieldDamage;
            amount -= shieldDamage; // Reduce the remaining damage that will go to health
            damageToShield = shieldDamage;
        }

        // Then, apply any remaining damage to health.
        if (amount > 0)
        {
            currentHealth -= amount;
            damageToHealth = amount;
        }

        Debug.Log($"Damage absorbed by shield: {damageToShield}. Damage dealt to health: {damageToHealth}.");
        Debug.Log($"--- NEW STATS -> [Shield: {currentShield}, Health: {currentHealth}] ---");

        // Update the UI with the new values.
        UpdateUI();

        // Check for death after all damage is calculated.
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
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
        // --- ADD THIS LINE ---
        // Tell the central GameManager to trigger the game over sequence.
        GameManager.Instance.TriggerGameOver();
    }
}