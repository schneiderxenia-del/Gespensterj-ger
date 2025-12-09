using UnityEngine;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/HideAfterTime")]
public class HideAfterTime : MonoBehaviour
{
    [Header("Wartezeit, bevor das Canvas ausgeblendet wird")]
    public float hideTime = 5f;                        // Zeit bis zum Ausblenden

    [Header("GhostManager")]
    public GhostManager ghostManager;                   // Referenz zum GhostManager (Start des Spawnens)

    void Start()
    {
        // Startet einen Timer, der nach hideTime die Methode HidePopup aufruft
        Invoke(nameof(HidePopup), hideTime);
    }

    // Blendet das UI aus und startet das Spawnen der Geister
    void HidePopup()
    {
        // Startet die Spawn-Logik, wenn ein GhostManager vorhanden ist
        if (ghostManager != null)
        {
            ghostManager.StartSpawning();
        }
        else
        {
            Debug.LogWarning("HideAfterTime: GhostManager ist nicht zugewiesen!");
        }

        // Canvas deaktivieren
        gameObject.SetActive(false);
    }
}
