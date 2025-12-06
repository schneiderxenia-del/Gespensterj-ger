using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerVR : MonoBehaviour
{
    public static GameManagerVR Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text levelText;
    public GameObject gameOverCanvas;


    int score = 0;
    int level = 1;
    public bool isGameOver { get; private set; } = false;

    const string HIGHSCORE_KEY = "HighscoreVR";

    void Awake()
    {
        Instance = this;

        int stored = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        highscoreText.text = "Highscore: " + stored;
        levelText.text = "Level: 1";

        gameOverCanvas.SetActive(false);
    }

    // SCORE
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        scoreText.text = "Score: " + score;
        CheckLevelUp();

        int stored = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        if (score > stored)
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, score);
            highscoreText.text = "Highscore: " + score;
        }
    }

    // LEVEL
    void CheckLevelUp()
    {
        int newLevel = (score / 10) + 1;

        if (newLevel != level)
        {
            level = newLevel;
            levelText.text = "Level: " + level;

            GhostDirector gd = FindFirstObjectByType<GhostDirector>();
            if (gd != null)
                gd.levelMultiplier = level;
        }
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;

        // ALLE Geister entfernen
        foreach (var ghost in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            Destroy(ghost);
        }

        // Game Over anzeigen
        gameOverCanvas.SetActive(true);

        // Zeit optional anhalten
        Time.timeScale = 0f;
    }



    // RESTART
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // BEENDEN
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
