using UnityEngine;

public class FogMonster : MonoBehaviour
{
    [Header("Reveal")]
    public GunLightController gunLight;
    public Transform revealPoint;
    public Renderer[] renderers;
    public GameObject revealedOnlyRoot;

    [Header("Health")]
    public int maxHealth = 2;
    public GameObject deathEffectPrefab;

    public bool IsRevealed { get; private set; }
    public Transform RevealPoint => revealPoint != null ? revealPoint : transform;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (revealPoint == null)
        {
            revealPoint = transform;
        }

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
    }

    private void Start()
    {
        if (gunLight == null)
        {
            gunLight = FindObjectOfType<GunLightController>();
        }

        SetRevealed(false);
    }

    private void Update()
    {
        bool lit = gunLight != null && gunLight.IsTargetLit(RevealPoint);
        SetRevealed(lit);
    }

    public void TakeDamage(int damage)
    {
        if (!IsRevealed)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void SetRevealed(bool revealed)
    {
        if (IsRevealed == revealed)
        {
            return;
        }

        IsRevealed = revealed;

        foreach (Renderer monsterRenderer in renderers)
        {
            if (monsterRenderer != null)
            {
                monsterRenderer.enabled = revealed;
            }
        }

        if (revealedOnlyRoot != null)
        {
            revealedOnlyRoot.SetActive(revealed);
        }
    }

    private void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterMonsterKill();
        }

        Destroy(gameObject);
    }
}
