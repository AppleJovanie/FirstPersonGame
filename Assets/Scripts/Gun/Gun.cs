using System.Collections;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    public AmmoType ammoType;
    public float damage = 10f;
    public float range = 100f;
    public int pelletsPerShot = 1;
    public float spreadAngle = 0f;
    public int clipSize = 12;
    private int currentAmmoInClip;
    public int maxReserveAmmo = 60;
    private int currentReserveAmmo;

    [Header("Reloading")]
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Visuals & Audio")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffectPrefab;
    public AudioSource audioSource;
    public AudioClip gunshotSound;
    public AudioClip reloadSound;

    [Header("References")]
    public Camera playerCamera;
    private TextMeshProUGUI ammoText;

    public void UpdateCameraReference(Camera newCamera)
    {
        playerCamera = newCamera;
        Debug.Log($"Gun's camera reference updated to: {newCamera.name}");
    }

    void Awake()
    {
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

    public void Initialize(TextMeshProUGUI ammoTextUI)
    {
        ammoText = ammoTextUI;
        if (ammoText != null)
        {
            ammoText.enabled = true;
            UpdateAmmoUI();
        }
    }

    void Update()
    {
        // Check for menus being open
        if (PauseMenu.isPaused) { return; }
        if (QuizManager.IsQuizActive) { return; }
        if (InventoryManager.IsMenuOpen) { return; }
        if (isReloading) { return; }

        // Check for shooting input
        if (Input.GetMouseButtonDown(0)) // Left Mouse Button
        {
            Debug.Log("--- Fire Button Pressed! ---");

            // --- Diagnostic Checks ---
            if (playerCamera == null)
            {
                Debug.LogError("SHOOT FAILED: The 'playerCamera' reference is NULL.");
                return;
            }
            if (ammoText == null)
            {
                Debug.LogError("SHOOT FAILED: The 'ammoText' reference is NULL.");
                return;
            }
            // --- End of Checks ---

            Debug.Log("SUCCESS: All references are valid. Calling Shoot().");
            Shoot();
        }

        // Check for reloading input
        if (Input.GetMouseButtonDown(1) && currentAmmoInClip < clipSize && currentReserveAmmo > 0) // Right Mouse Button
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        yield return new WaitForSeconds(reloadTime);
        int ammoNeeded = clipSize - currentAmmoInClip;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);
        currentAmmoInClip += ammoToReload;
        currentReserveAmmo -= ammoToReload;
        UpdateAmmoUI();
        isReloading = false;
    }

    void Shoot()
    {
        if (isReloading || currentAmmoInClip <= 0)
        {
            if (currentAmmoInClip <= 0 && currentReserveAmmo > 0) StartCoroutine(Reload());
            return;
        }
        currentAmmoInClip--;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && gunshotSound != null) audioSource.PlayOneShot(gunshotSound);

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

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );
            Vector3 finalDirection = spreadRotation * direction;
            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(finalDirection));

            Bullet bulletScript = bulletObject.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(damage);
                bulletScript.impactEffect = impactEffectPrefab;
            }
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
            ammoText.text = $"{currentAmmoInClip} / {currentReserveAmmo}";
        }
    }

    public void AddReserveAmmo(int amount)
    {
        currentReserveAmmo += amount;
        UpdateAmmoUI();
    }
}

