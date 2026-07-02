using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;

    public int CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);
        UpdateUI();

        if (CurrentHealth <= 0 && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void UpdateUI()
    {
        if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
        {
            GameManager.Instance.uiManager.UpdateHealthText(CurrentHealth, maxHealth);
        }
    }
}
