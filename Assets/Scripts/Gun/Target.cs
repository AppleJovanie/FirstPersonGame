using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{transform.name} took {amount} damage. Health is now {health}.");

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{transform.name} has been destroyed.");
        Destroy(gameObject);
    }
}
