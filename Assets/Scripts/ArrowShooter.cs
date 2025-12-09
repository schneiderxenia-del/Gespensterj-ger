using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/ArrowShooter")]
public class ArrowShooter : MonoBehaviour
{
    [Header("Einstellungen für Pfeilabschuss")]
    [SerializeField] private float launchSpeed = 10f;         // Grundgeschwindigkeit des Pfeils
    [SerializeField] private GameObject trailEffect;          // Aktiviert nach dem Abschuss

    private Rigidbody rb;                                     // Physik des Pfeils
    private bool isFlying = false;                            // Ob der Pfeil bereits fliegt
    private BowStringPull bowString;                          // Verknüpfte Sehne

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError($"Arrow '{name}' benötigt einen Rigidbody!");

        TogglePhysics(false);                                 // Pfeil startet statisch
    }

    // Verbindet den Pfeil mit der Sehne, um den Abschuss zu erkennen
    public void BindToString(BowStringPull pull)
    {
        bowString = pull;
        bowString.OnReleased += LaunchArrow;
    }

    private void OnDestroy()
    {
        if (bowString != null)
            bowString.OnReleased -= LaunchArrow;
    }

    // Wird ausgelöst, wenn der Spieler die Sehne loslässt
    private void LaunchArrow(float pullStrength)
    {
        if (bowString != null)
            bowString.OnReleased -= LaunchArrow;

        transform.parent = null;     // Pfeil vom Bogen lösen
        isFlying = true;

        TogglePhysics(true);

        // Pfeil abschießen
        Vector3 force = transform.forward * pullStrength * launchSpeed;
        rb.AddForce(force, ForceMode.Impulse);

        // Pfeil rotiert während des Flugs zur Flugbahn
        StartCoroutine(AlignToVelocity());

        // Spur aktivieren
        if (trailEffect != null)
            trailEffect.SetActive(true);
    }

    // Lässt den Pfeil immer der Flugrichtung folgen (Wurfgeschossverhalten)
    private IEnumerator AlignToVelocity()
    {
        yield return new WaitForFixedUpdate();

        while (isFlying)
        {
            if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation =
                    Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
            }

            yield return null;
        }
    }

    // Stoppt den Flug (z. B. bei Einschlag)
    public void StopArrow()
    {
        isFlying = false;
        TogglePhysics(false);

        if (trailEffect != null)
            trailEffect.SetActive(false);
    }

    // Aktiviert oder deaktiviert Physik am Pfeil
    private void TogglePhysics(bool value)
    {
        rb.useGravity = value;
        rb.isKinematic = !value;
    }
}
