using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/ArrowSpawner")]
public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;   // Pfeil, der gespawnt wird
    [SerializeField] private GameObject notchPoint;    // Position am Bogen, wo der Pfeil eingehängt sitzt
    [SerializeField] private float spawnDelay = 1f;    // Verzögerung für neuen Pfeil

    private XRGrabInteractable bow;                    // Bogen, der gehalten wird
    private BowStringPull bowStringPull;               // Sehne, erkennt Abschuss
    private bool arrowNotched = false;                 // Ob ein Pfeil eingespannt ist
    private GameObject currentArrow = null;            // Der aktive Pfeil

    private float spawnTimer = 0f;
    private bool waitingForSpawn = false;

    private void Start()
    {
        bow = GetComponent<XRGrabInteractable>();
        bowStringPull = GetComponentInChildren<BowStringPull>();

        // NEUE EVENTS — passend zur neuen BowStringPull-Klasse!
        if (bowStringPull != null)
            bowStringPull.OnReleased += NotchEmpty;
    }

    private void OnDestroy()
    {
        if (bowStringPull != null)
            bowStringPull.OnReleased -= NotchEmpty;
    }

    private void Update()
    {
        HandleSpawnState();
        HandleReleaseState();
        TickSpawnTimer();
    }

    // Startet Timer, sobald der Bogen gehalten wird und kein Pfeil eingespannt ist
    private void HandleSpawnState()
    {
        if (bow.isSelected && !arrowNotched && !waitingForSpawn)
        {
            waitingForSpawn = true;
            spawnTimer = spawnDelay;
        }
    }

    // Wenn Bogen losgelassen wird → eingespannten Pfeil entfernen
    private void HandleReleaseState()
    {
        if (!bow.isSelected && currentArrow != null)
        {
            Destroy(currentArrow);
            NotchEmpty(1f);
        }
    }

    // Timer runterzählen → Pfeil erzeugen
    private void TickSpawnTimer()
    {
        if (!waitingForSpawn)
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            waitingForSpawn = false;
            CreateArrow();
        }
    }

    // Erstellt einen neuen Pfeil und verbindet ihn mit der BowStringPull-Klasse
    private void CreateArrow()
    {
        arrowNotched = true;

        currentArrow = Instantiate(arrowPrefab, notchPoint.transform);

        ArrowShooter shooter = currentArrow.GetComponent<ArrowShooter>();
        if (shooter != null && bowStringPull != null)
        {
            shooter.BindToString(bowStringPull);  // Neue Methode für Verbindung
        }
    }

    // Wird ausgelöst, wenn die Sehne losgelassen wurde
    private void NotchEmpty(float value)
    {
        arrowNotched = false;
        currentArrow = null;
    }
}
