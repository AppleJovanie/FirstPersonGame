using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;        // Bullet travel speed
    public float lifeTime = 3f;       // Time before auto-destroy
    public GameObject impactEffect;   // Explosion / impact prefab

    private float damage;             // Damage value set by Gun script
    private Rigidbody rb;             // Rigidbody reference

    void Start()
    {
        // Get Rigidbody and push the bullet forward immediately
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // No drop unless you want realistic physics
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.velocity = transform.forward * speed; // move bullet
        }

        // Auto destroy after lifetime
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Instantiate an impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Check if target can take damage
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Destroy bullet
        Destroy(gameObject);
    }
}
