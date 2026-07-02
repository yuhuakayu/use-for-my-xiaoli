using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public Text healthText;
    public Text monsterText;
    public Text ammoText;
    public Text lightText;
    public Text messageText;

    [Header("End Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public Button restartButton;

    private Coroutine messageRoutine;

    private void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (messageText != null)
        {
            messageText.text = "";
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + " / " + maxHealth;
        }
    }

    public void UpdateMonsterText(int currentCount, int targetCount)
    {
        if (monsterText != null)
        {
            monsterText.text = "Monsters: " + currentCount + " / " + targetCount;
        }
    }

    public void UpdateAmmoText(int currentAmmo, int reserveAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo + " / " + reserveAmmo;
        }
    }

    public void UpdateLightText(bool isOn)
    {
        if (lightText != null)
        {
            lightText.text = isOn ? "Light: ON" : "Light: OFF";
        }
    }

    public void ShowTemporaryMessage(string message)
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageRoutine = StartCoroutine(MessageRoutine(message));
    }

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        ShowMessage("Fog cleared. All monsters eliminated.");
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        ShowMessage("You were taken by the fog.");
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    private IEnumerator MessageRoutine(string message)
    {
        ShowMessage(message);
        yield return new WaitForSecondsRealtime(2f);
        ShowMessage("");
    }
}
