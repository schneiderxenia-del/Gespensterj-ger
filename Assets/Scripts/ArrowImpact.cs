using UnityEngine;
using System.Collections;

// Steuert das Verhalten des Pfeils nach einer Kollision.
// Der Pfeil bleibt im getroffenen Objekt stecken und wird nach einer Zeit entfernt.
public class ArrowImpact : MonoBehaviour
{
    [Header("Einstellungen für Pfeil-Einschlag")]
    [SerializeField] private float stickDuration = 3f;     // Zeit, bis der Pfeil gelöscht wird
    [SerializeField] private float minDepth = 0.05f;       // Mindesttiefe des Eindringens
    [SerializeField] private float maxDepth = 0.15f;       // Maximale Eindringtiefe
    [SerializeField] private LayerMask ignoreLayers;       // Layer, die keine Kollision auslösen sollen
    [SerializeField] private Transform arrowTip;           // Transform der Pfeilspitze

    private ArrowShooter arrowShooter; // Referenz zum Schussskript
    private Rigidbody rigidbody;         // Rigidbody des Pfeils
    private bool hasHit = false;         // True, sobald der Pfeil einmal aufgeschlagen ist

    private void Awake()
    {
        // Komponentenreferenzen holen
     arrowShooter = GetComponent<ArrowShooter>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Keine weitere Verarbeitung, wenn bereits getroffen oder Layer ignoriert werden soll
        if (hasHit || ((1 << collision.gameObject.layer) & ignoreLayers) != 0)
            return;

        hasHit = true;

        // Flug stoppen, damit der Pfeil nicht weiter bewegt wird
     arrowShooter.StopArrow();

        HandleStick(collision);
    }

    // Setzt den Pfeil an der Einschlagstelle fest und richtet ihn korrekt aus
    private void HandleStick(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        // Pfeil behält seine Flugrichtung für eine realistische Ausrichtung
        Vector3 direction = transform.forward;
        Vector3 up = transform.up;

        // Zufällige Eindringtiefe erzeugt natürlicheres Verhalten
        float depth = Random.Range(minDepth, maxDepth);

        // Pfeilrotation an Einschlagwinkel anpassen
        Quaternion rotation = Quaternion.LookRotation(direction, up);

        // Pfeilspitze korrekt in das Objekt schieben
        Vector3 tipOffset = arrowTip.localPosition;
        Vector3 position =
            contact.point - (rotation * tipOffset) + contact.normal * -depth;

        transform.SetPositionAndRotation(position, rotation);

        // Physikalische Verbindung zum Objekt herstellen
        CreateEmbedJoint(collision, depth);

        // Pfeil wird zum Kind des getroffenen Objekts
        transform.SetParent(collision.transform, true);

        // Pfeil nach Zeit automatisch entfernen
        StartCoroutine(DespawnAfterDelay());
    }

    // Verhindert, dass der Pfeil aus dem Ziel herausfällt
    private ConfigurableJoint CreateEmbedJoint(Collision collision, float depth)
    {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = collision.rigidbody;

        // Bewegungen beschränken, damit der Pfeil stabil sitzt
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Eindringtiefe als Limit setzen
        SoftJointLimit limit = joint.linearLimit;
        limit.limit = depth;
        joint.linearLimit = limit;

        return joint;
    }

    // Löscht den Pfeil nach Ablauf der Stick-Dauer
    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(stickDuration);
        Destroy(gameObject);
    }
}

