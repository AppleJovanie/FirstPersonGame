using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 2f;
    public GameObject impactEffect;

    private float damage;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.velocity = transform.forward * speed;
        }
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    // --- THIS IS THE CORRECTED METHOD ---
    // We use OnCollisionEnter for solid objects that should collide and stop.
    private void OnCollisionEnter(Collision collision)
    {
        // --- DEBUGGING LOG ---
        // This will print the name of any object the bullet hits to the console.
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Check if the hit object can take damage
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Destroy the bullet on impact
        Destroy(gameObject);
    }
}
