using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/GameManager")]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;        // Singleton für globalen Zugriff

    [Header("UI")]
    public TMP_Text scoreText;                // Anzeige des aktuellen Scores
    public TMP_Text highscoreText;            // Anzeige des Highscores
    public TMP_Text levelText;                // Anzeige des aktuellen Levels
    public GameObject gameOverCanvas;         // Game-Over-Bildschirm

    int score = 0;                            // Aktueller Punktestand
    int level = 1;                            // Aktuelles Level
    public bool isGameOver { get; private set; } = false; // Spielzustand

    const string HIGHSCORE_KEY = "Highscore";  // PlayerPrefs-Schlüssel

    private void Awake()
    {
        // Singleton setzen
        Instance = this;

        // Highscore laden
        int stored = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        highscoreText.text = "Highscore: " + stored;

        // Levelanzeige initialisieren
        levelText.text = "Level: 1";

        // Game-Over-Bildschirm ausblenden
        gameOverCanvas.SetActive(false);
    }

    // Fügt Punkte hinzu und prüft Level-Aufstieg
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        scoreText.text = "Score: " + score;

        CheckLevelUp();

        // Highscore aktualisieren
        int stored = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        if (score > stored)
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, score);
            highscoreText.text = "Highscore: " + score;
        }
    }

    // Bestimmt, wann der Spieler ein neues Level erreicht
    private void CheckLevelUp()
    {
        int newLevel = (score / 10) + 1;

        if (newLevel != level)
        {
            level = newLevel;
            levelText.text = "Level: " + level;

            // Ghost-Spawn-Rate anpassen
            GhostManager ghostManager = FindFirstObjectByType<GhostManager>();
            if (ghostManager != null)
                ghostManager.levelMultiplier = level;
        }
    }

    // Wird aufgerufen, sobald der Spieler stirbt
    public void PlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Alle Geister aus der Szene entfernen
        foreach (var ghost in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            Destroy(ghost);
        }

        // Game Over anzeigen
        gameOverCanvas.SetActive(true);

        // Spiel pausieren
        Time.timeScale = 0f;
    }

    // Neustart der aktuellen Szene
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Spiel schließen
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
