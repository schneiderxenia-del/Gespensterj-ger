using UnityEngine;
using System.Collections.Generic;

public class GhostDirector : MonoBehaviour
{
    [Header("Surface & Prefab")]
    public Collider surfaceCollider;
    public GameObject ghostPrefab;

    [Header("Spawn-Logik")]
    public float minEdgeDistance = 0.3f;
    public float extraOffsetAboveSurface = 0.2f;
    public float minSpacing = 0.3f;
    public LayerMask overlapCheckLayers = ~0;

    [Header("Limits & Schwierigkeit")]
    public int baseMaxAlive = 4;                    // Start
    public int maxAliveIncreasePerMinute = 2;       // wird pro Minute erhöht
    public float spawnIntervalAtStart = 2.5f;       // Start-Intervall
    public float spawnIntervalAtMax = 0.7f;         // schneller im Late-Game
    public float difficultyRampMinutes = 5f;        // nach so vielen Minuten gilt "Max"

    [Header("Geist-Setup")]
    public Transform playerHead;                    // HMD/Kamera → wird an GhostAI gesetzt

    // NEU: Steuerung durch Canvas
    [Header("Spawn aktiv?")]
    public bool spawningEnabled = false;

    [Header("Level-Multiplikator (von GameManagerVR gesetzt)")]
    public int levelMultiplier = 1;

    float _timer;
    float _elapsed;
    readonly List<GameObject> _alive = new();

    // -----------------------------------------------------------
    // NEU: Wird vom Canvas aufgerufen
    // -----------------------------------------------------------
    public void StartSpawning()
    {
        spawningEnabled = true;
    }

    void Update()
    {
        // NEU: Spawnen erst nach Canvas-Freigabe
        if (!spawningEnabled) return;

        if (!surfaceCollider || !ghostPrefab) return;

        if (GameManagerVR.Instance != null && GameManagerVR.Instance.isGameOver)
            return;


        _elapsed += Time.deltaTime;

        // Dynamische Limits / Rate
        float t = Mathf.Clamp01(_elapsed / (difficultyRampMinutes * 60f));
        int curMaxAlive = baseMaxAlive + Mathf.RoundToInt(t * (maxAliveIncreasePerMinute * difficultyRampMinutes));
        float interval = Mathf.Lerp(spawnIntervalAtStart, spawnIntervalAtMax, t);

        // Level = schneller spawnen
        interval /= Mathf.Max(1, levelMultiplier);


        // Tote aus Liste räumen
        _alive.RemoveAll(go => go == null);

        // Spawnen, wenn wir unter Limit sind
        _timer += Time.deltaTime;
        if (_alive.Count < curMaxAlive && _timer >= interval)
        {
            if (TrySpawn(out GameObject g))
            {
                _alive.Add(g);
            }
            _timer = 0f;
        }
    }

    bool TrySpawn(out GameObject spawned)
    {
        spawned = null;

        // Prefab-Höhe
        float prefabHalfHeight = 0.1f;
        var r = ghostPrefab.GetComponentInChildren<Renderer>();
        if (r) prefabHalfHeight = Mathf.Max(0.05f, r.bounds.extents.y);

        // Mehrere Versuche, eine freie Stelle zu finden
        const int triesMax = 200;
        int tries = 0;
        while (tries++ < triesMax)
        {
            if (!TryGetRandomHitOnSurface(out RaycastHit hit)) continue;

            Vector3 pos = hit.point + hit.normal * (prefabHalfHeight + extraOffsetAboveSurface);
            float checkRadius = Mathf.Max(minSpacing, prefabHalfHeight * 0.9f);
            bool overlaps = Physics.CheckSphere(pos, checkRadius, overlapCheckLayers, QueryTriggerInteraction.Ignore);
            if (overlaps) continue;

            // Rotation: nach „vorn“ (Player-Richtung) blicken
            Vector3 fwd = (playerHead ? (playerHead.position - pos) : Vector3.forward);
            fwd = Vector3.ProjectOnPlane(fwd, hit.normal).normalized;
            if (fwd.sqrMagnitude < 1e-3f) fwd = Vector3.forward;
            Quaternion rot = Quaternion.LookRotation(fwd, hit.normal);

            spawned = Instantiate(ghostPrefab, pos, rot);

            // Ziel an GhostAI übergeben
            var ai = spawned.GetComponent<GhostAI>();
            if (ai)
            {
                ai.target = playerHead ? playerHead : (Camera.main ? Camera.main.transform : null);
            }
            return true;
        }
        return false;
    }

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
    void OnDrawGizmosSelected()
    {
        if (!surfaceCollider) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(surfaceCollider.bounds.center, surfaceCollider.bounds.size);
    }
#endif
}

