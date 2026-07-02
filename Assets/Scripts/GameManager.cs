using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Victory,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Goal")]
    public int monstersToKill = 6;

    [Header("References")]
    public UIManager uiManager;

    public int MonstersKilled { get; private set; }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        UpdateProgressUI();
    }

    public void RegisterMonsterKill()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        MonstersKilled = Mathf.Clamp(MonstersKilled + 1, 0, monstersToKill);
        UpdateProgressUI();

        if (MonstersKilled >= monstersToKill)
        {
            WinGame();
        }
    }

    public void GameOver()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
    }

    public void WinGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Victory;
        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowVictory();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateProgressUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateMonsterText(MonstersKilled, monstersToKill);
        }
    }
}
