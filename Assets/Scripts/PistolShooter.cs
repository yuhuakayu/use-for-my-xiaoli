using UnityEngine;

public class PistolShooter : MonoBehaviour
{
    [Header("Aiming")]
    public Camera aimCamera;
    public GunLightController gunLight;
    public LayerMask hitMask = ~0;
    public float range = 35f;

    [Header("Weapon")]
    public int damage = 1;
    public int magazineSize = 8;
    public int reserveAmmo = 24;
    public float fireCooldown = 0.22f;
    public float reloadTime = 1.1f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource shotAudio;
    public AudioSource reloadAudio;

    private int currentAmmo;
    private bool isReloading;
    private float nextFireTime;
    private float reloadEndTime;

    private void Start()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

        if (gunLight == null)
        {
            gunLight = GetComponentInChildren<GunLightController>();
        }

        currentAmmo = magazineSize;
        UpdateUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        if (isReloading && Time.time >= reloadEndTime)
        {
            FinishReload();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (isReloading || Time.time < nextFireTime)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + fireCooldown;

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (shotAudio != null)
        {
            shotAudio.Play();
        }

        ShootRay();
        UpdateUI();
    }

    private void ShootRay()
    {
        if (aimCamera == null)
        {
            return;
        }

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        FogMonster monster = hit.collider.GetComponentInParent<FogMonster>();
        if (monster == null)
        {
            return;
        }

        if (gunLight != null && !gunLight.IsTargetLit(monster.RevealPoint))
        {
            if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
            {
                GameManager.Instance.uiManager.ShowTemporaryMessage("Keep it inside the gun light.");
            }

            return;
        }

        monster.TakeDamage(damage);
    }

    private void StartReload()
    {
        if (isReloading || currentAmmo >= magazineSize || reserveAmmo <= 0)
        {
            return;
        }

        isReloading = true;
        reloadEndTime = Time.time + reloadTime;

        if (reloadAudio != null)
        {
            reloadAudio.Play();
        }
    }

    private void FinishReload()
    {
        int neededAmmo = magazineSize - currentAmmo;
        int loadedAmmo = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += loadedAmmo;
        reserveAmmo -= loadedAmmo;
        isReloading = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
        {
            GameManager.Instance.uiManager.UpdateAmmoText(currentAmmo, reserveAmmo);
        }
    }
}
