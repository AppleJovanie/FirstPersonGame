using System.Collections;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 10f;
    public int clipSize = 12;
    private int currentAmmoInClip;
    public int maxReserveAmmo = 60;
    private int currentReserveAmmo;

    [Header("Reloading")]
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f;

    [Header("References")]
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffectPrefab;
    public AudioSource audioSource;
    public AudioClip gunshotSound;
    public AudioClip reloadSound;

    // This will be found automatically at runtime
    private TextMeshProUGUI ammoText;

    void Start()
    {
        currentAmmoInClip = clipSize;
        currentReserveAmmo = maxReserveAmmo; // Start with full reserve ammo

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Find the Ammo UI Text in the scene by its tag
        GameObject ammoTextObject = GameObject.FindGameObjectWithTag("AmmoUI");
        if (ammoTextObject != null)
        {
            ammoText = ammoTextObject.GetComponent<TextMeshProUGUI>();
            ammoText.enabled = true; // Enable the text component
            UpdateAmmoUI();
        }
        else
        {
            Debug.LogWarning("Could not find object with 'AmmoUI' tag. Ammo display will not work.");
        }
    }

    void OnDestroy()
    {
        // Hide the ammo UI when this gun is unequipped/destroyed
        if (ammoText != null)
        {
            ammoText.enabled = false;
        }
    }

    void Update()
    {
        // Prevent any actions if we are currently reloading
        if (isReloading)
        {
            return;
        }

        // Left-click to shoot
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Right-click to manually reload, but only if the clip isn't full and we have reserve ammo
        if (Input.GetMouseButtonDown(1) && currentAmmoInClip < clipSize && currentReserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Play reload sound if available
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        // Wait for the reload time to finish
        yield return new WaitForSeconds(reloadTime);

        // Calculate how much ammo is needed and how much is available to reload
        int ammoNeeded = clipSize - currentAmmoInClip;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);

        // Add ammo to the clip and remove it from the reserve
        currentAmmoInClip += ammoToReload;
        currentReserveAmmo -= ammoToReload;

        UpdateAmmoUI();

        isReloading = false;
    }

    void Shoot()
    {
        // Check if we have ammo in the clip or are in the middle of reloading
        if (currentAmmoInClip <= 0 || isReloading)
        {
            // Optionally, play an empty clip sound here
            return;
        }

        currentAmmoInClip--;
        UpdateAmmoUI();

        // Play muzzle flash and gunshot sound if available
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && gunshotSound != null) audioSource.PlayOneShot(gunshotSound);

        // Create the bullet
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Pass necessary info to the bullet
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
            bulletScript.impactEffect = impactEffectPrefab;
        }

        // Give the bullet its velocity
        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        // --- NEW: Automatically reload if the clip is now empty ---
        if (currentAmmoInClip <= 0 && currentReserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            // Display both the clip ammo and the total reserve ammo
            ammoText.text = $"{currentAmmoInClip} / {currentReserveAmmo}";
        }
    }
}
