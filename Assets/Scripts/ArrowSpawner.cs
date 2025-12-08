using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject notchPoint;
    [SerializeField] private float spawnDelay = 1f;

    private XRGrabInteractable bow;
    private XRPullInteractable pullInteractable;
    private bool arrowNotched = false;
    private GameObject currentArrow = null;

    private float spawnTimer = 0f;
    private bool waitingForSpawn = false;

    private void Start()
    {
        bow = GetComponent<XRGrabInteractable>();
        pullInteractable = GetComponentInChildren<XRPullInteractable>();

        if (pullInteractable != null)
            pullInteractable.PullActionReleased += NotchEmpty;
    }

    private void OnDestroy()
    {
        if (pullInteractable != null)
            pullInteractable.PullActionReleased -= NotchEmpty;
    }

    private void Update()
    {
        HandleSpawnState();
        HandleReleaseState();
        TickSpawnTimer();
    }

    private void HandleSpawnState()
    {
        if (bow.isSelected && !arrowNotched && !waitingForSpawn)
        {
            waitingForSpawn = true;
            spawnTimer = spawnDelay;
        }
    }

    private void HandleReleaseState()
    {
        if (!bow.isSelected && currentArrow != null)
        {
            Destroy(currentArrow);
            NotchEmpty(1f);
        }
    }

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

    private void CreateArrow()
    {
        arrowNotched = true;

        currentArrow = Instantiate(arrowPrefab, notchPoint.transform);

        ArrowLauncher launcher = currentArrow.GetComponent<ArrowLauncher>();
        if (launcher != null && pullInteractable != null)
        {
            launcher.Initialize(pullInteractable);
        }
    }

    private void NotchEmpty(float value)
    {
        arrowNotched = false;
        currentArrow = null;
    }
}
