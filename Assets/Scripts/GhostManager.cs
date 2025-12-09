using UnityEngine;
using System.Collections.Generic;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/GhostManager")]

// Verwaltet alle Geister: Spawnen, Schwierigkeitsanstieg, Alive-Limits.
public class GhostManager : MonoBehaviour
{
    [Header("Surface & Prefab")]
    public Collider surfaceCollider;        // Fläche, auf der Geister erscheinen dürfen
    public GameObject ghostPrefab;          // Geist-Prefab, das gespawnt wird

    [Header("Spawn-Logik")]
    public float minEdgeDistance = 0.3f;    // Abstand zum Rand der Spawnfläche
    public float extraOffsetAboveSurface = 0.2f; // Abstand über der Fläche (Hover)
    public float minSpacing = 0.3f;         // Mindestabstand zwischen zwei Geistern
    public LayerMask overlapCheckLayers = ~0; // Layer für Abstandskontrolle

    [Header("Limits & Schwierigkeit")]
    public int baseMaxAlive = 4;            // Startlimit der lebenden Geister
    public int maxAliveIncreasePerMinute = 2; // Schwierigkeitserhöhung pro Minute
    public float spawnIntervalAtStart = 2.5f; // Spawnrate zu Beginn
    public float spawnIntervalAtMax = 0.7f;   // Schnellste Spawnrate
    public float difficultyRampMinutes = 5f;  // Zeit bis maximale Schwierigkeit erreicht ist

    [Header("Geist-Setup")]
    public Transform playerHead;            // Ziel für Geister (z. B. Kamera)

    [Header("Spawn aktiv?")]
    public bool spawningEnabled = false;    // Wird erst aktiv durch StartSpawning()

    [Header("Level-Multiplikator (von GameManager gesetzt)")]
    public int levelMultiplier = 1;         // Erhöht die Spawnrate abhängig vom Level

    float _timer;                           // Zeit seit letztem Spawnversuch
    float _elapsed;                         // Zeit seit Beginn des Spawnens
    readonly List<GameObject> _alive = new(); // Liste aller lebenden Geister


    // Aktiviert das Spawnen
    public void StartSpawning()
    {
        spawningEnabled = true;
    }

    void Update()
    {
        if (!spawningEnabled) return;
        if (!surfaceCollider || !ghostPrefab) return;

        // Bei Game Over → kein Spawnen mehr
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        _elapsed += Time.deltaTime;

        // Lineare Schwierigkeitserhöhung über mehrere Minuten
        float t = Mathf.Clamp01(_elapsed / (difficultyRampMinutes * 60f));

        // Berechne maximale Geisteranzahl basierend auf Zeit
        int curMaxAlive =
            baseMaxAlive + Mathf.RoundToInt(t * (maxAliveIncreasePerMinute * difficultyRampMinutes));

        // Spawnrate interpoliert zwischen langsam → schnell
        float interval = Mathf.Lerp(spawnIntervalAtStart, spawnIntervalAtMax, t);

        // Level beeinflusst Spawnfrequenz
        interval /= Mathf.Max(1, levelMultiplier);

        // Entferne zerstörte Einträge
        _alive.RemoveAll(go => go == null);

        _timer += Time.deltaTime;

        // Darf gespawnt werden?
        if (_alive.Count < curMaxAlive && _timer >= interval)
        {
            if (TrySpawn(out GameObject g))
            {
                _alive.Add(g);
            }
            _timer = 0f;
        }
    }

    // Versucht, einen neuen Geist validiert zu platzieren
    bool TrySpawn(out GameObject spawned)
    {
        spawned = null;

        float prefabHalfHeight = 0.1f;

        // Höhe des Prefabs bestimmen
        var r = ghostPrefab.GetComponentInChildren<Renderer>();
        if (r) prefabHalfHeight = Mathf.Max(0.05f, r.bounds.extents.y);

        const int triesMax = 200;
        int tries = 0;

        while (tries++ < triesMax)
        {
            // Zufälligen Punkt auf der Oberfläche suchen
            if (!TryGetRandomHitOnSurface(out RaycastHit hit)) continue;

            // Zielposition über der Fläche
            Vector3 pos = hit.point + hit.normal * (prefabHalfHeight + extraOffsetAboveSurface);

            // Prüfen, ob dort genug Platz ist
            float checkRadius = Mathf.Max(minSpacing, prefabHalfHeight * 0.9f);
            bool overlaps = Physics.CheckSphere(pos, checkRadius, overlapCheckLayers, QueryTriggerInteraction.Ignore);
            if (overlaps) continue;

            // Geist soll zum Spieler schauen
            Vector3 fwd = (playerHead ? (playerHead.position - pos) : Vector3.forward);
            fwd = Vector3.ProjectOnPlane(fwd, hit.normal).normalized;
            if (fwd.sqrMagnitude < 1e-3f) fwd = Vector3.forward;

            Quaternion rot = Quaternion.LookRotation(fwd, hit.normal);

            // Geist erzeugen
            spawned = Instantiate(ghostPrefab, pos, rot);

            // AI konfigurieren
            var ai = spawned.GetComponent<GhostAI>();
            if (ai)
            {
                ai.target = playerHead ? playerHead :
                           (Camera.main ? Camera.main.transform : null);
            }

            return true;
        }

        // Kein valider Spawnpunkt gefunden
        return false;
    }

    // Sucht zufälligen Punkt auf der Fläche mit Raycast
    bool TryGetRandomHitOnSurface(out RaycastHit hit)
    {
        Bounds b = surfaceCollider.bounds;

        float x = Random.Range(b.min.x + minEdgeDistance, b.max.x - minEdgeDistance);
        float z = Random.Range(b.min.z + minEdgeDistance, b.max.z - minEdgeDistance);

        Vector3 origin = new Vector3(x, b.max.y + 2f, z);
        Ray ray = new Ray(origin, Vector3.down);

        if (surfaceCollider.Raycast(ray, out hit, b.size.y + 4f))
            return true;

        return false;
    }

#if UNITY_EDITOR
    // Zeichnet die Spawnfläche im Editor
    void OnDrawGizmosSelected()
    {
        if (!surfaceCollider) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(surfaceCollider.bounds.center, surfaceCollider.bounds.size);
    }
#endif
}
