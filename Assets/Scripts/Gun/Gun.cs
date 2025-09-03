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

    // Ito ang bagong Shoot() method para sa Gun.cs
    void Shoot()
    {
        // --- Parehong logic para sa ammo at effects ---
        if (isReloading) return;

        if (currentAmmoInClip <= 0)
        {
            if (currentReserveAmmo > 0) StartCoroutine(Reload());
            return;
        }

        currentAmmoInClip--;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && gunshotSound != null) audioSource.PlayOneShot(gunshotSound);

        // --- BAGONG LOGIC PARA SA ACCURATE AIMING ---
        RaycastHit hit;
        Vector3 targetPoint;

        // Mag-cast tayo ng ray mula sa gitna ng camera
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
        {
            // Kung may tinamaan, iyon ang target point natin
            targetPoint = hit.point;
        }
        else
        {
            // Kung walang tinamaan (hal. nakatutok sa langit), gumawa tayo ng target point na malayo
            targetPoint = playerCamera.transform.position + playerCamera.transform.forward * 1000; // 1000 units away
        }

        // I-calculate ang direksyon mula sa dulo ng baril (firePoint) papunta sa targetPoint
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // Gumawa ng bala sa pwesto ng firePoint
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

        // --- Ang natitirang code ay pareho lang ---
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
            bulletScript.impactEffect = impactEffectPrefab;
        }

        // Ang bala ay lilipad na ngayon sa eksaktong direksyon ng crosshair
        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

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
