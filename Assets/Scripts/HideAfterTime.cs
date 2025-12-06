using UnityEngine;

public class HideAfterTime : MonoBehaviour
{
    [Header("Wartezeit, bevor das Canvas ausgeblendet wird")]
    public float hideTime = 5f;

    [Header("GhostDirector, der nach dem Ausblenden starten soll")]
    public GhostDirector ghostDirector;

    void Start()
    {
        // Nach X Sekunden HidePopup() aufrufen
        Invoke(nameof(HidePopup), hideTime);
    }

    void HidePopup()
    {
        // 1️⃣ Zuerst Geister-Spawning starten
        if (ghostDirector != null)
        {
            ghostDirector.StartSpawning();
        }
        else
        {
            Debug.LogWarning("HideAfterTime: GhostDirector ist nicht zugewiesen!");
        }

        // 2️⃣ Dann Canvas ausblenden
        gameObject.SetActive(false);
    }
}
