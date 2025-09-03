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
    public float range = 100f;
    public float spreadAngle = 5.0f;
    public int pelletsPerShot = 1;

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

    void Awake()
    {
        // Initialize ammo counts as soon as the gun is created.
        currentAmmoInClip = clipSize;
        currentReserveAmmo = maxReserveAmmo;
    }
    void Start()
    {
        

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

    }
    // This new public method will be called by the InventoryManager.
    public void Initialize(TextMeshProUGUI ammoTextUI)
    {
        ammoText = ammoTextUI;
        if (ammoText != null)
        {
            ammoText.enabled = true; // Enable the text component.
            UpdateAmmoUI();
        }
        else
        {
            Debug.LogWarning("QuizManager: Could not find 'AmmoUI' tag. Ammo display will not work.");
        }
    }



    void Update()
    {
        // --- NEW SAFETY CHECK ---
        // Only allow shooting/reloading if the gun is equipped (is a child of the player's hand).
        if (transform.parent == null || !transform.parent.CompareTag("PlayerHand"))
        {
            return; // Stop the rest of the Update method from running.
        }
        // --- END OF NEW CHECK ---

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

        // Right-click to manually reload
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

    // Ito ang bagong Shoot() method para sa Gun.cs
    // Inside Gun.cs

    // Inside Gun.cs

    void Shoot()
    {
        // --- Standard ammo check (This happens only ONCE per shot) ---
        if (isReloading) return;
        if (currentAmmoInClip <= 0)
        {
            if (currentReserveAmmo > 0) StartCoroutine(Reload());
            return;
        }
        currentAmmoInClip--; // Use one shell per shot
        UpdateAmmoUI();

        // --- Visual & Audio Effects (Also only ONCE per shot) ---
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && gunshotSound != null) audioSource.PlayOneShot(gunshotSound);

        // --- ACCURATE AIMING: Find the center target point ---
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = playerCamera.transform.position + playerCamera.transform.forward * range;
        }

        // --- SHOTGUN LOGIC: Fire multiple pellets in a loop ---
        for (int i = 0; i < pelletsPerShot; i++)
        {
            // Calculate the base direction from the gun barrel to the center target
            Vector3 direction = (targetPoint - firePoint.position).normalized;

            // --- Calculate Spread ---
            // Create a random rotation within the spread angle
            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );

            // Apply the random rotation to the base direction
            Vector3 finalDirection = spreadRotation * direction;

            // --- Create the Bullet ---
            // Create the bullet at the gun's fire point
            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(finalDirection));

            // Pass damage value to the bullet
            Bullet bulletScript = bulletObject.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(damage); // Each pellet does full damage in this setup
                bulletScript.impactEffect = impactEffectPrefab;
            }
        }

        // Auto-reload if the clip is now empty
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
    // Add this method to your Gun.cs script
    public void AddReserveAmmo(int amount)
    {
        currentReserveAmmo += amount;
        UpdateAmmoUI();
        Debug.Log($"Added {amount} ammo. New reserve: {currentReserveAmmo}");
    }
}
