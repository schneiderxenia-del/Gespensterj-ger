using UnityEngine;

[HelpURL("https://github.com/schneiderxenia-del/Gespensterj-ger/wiki/PlayerHitbox")]
public class PlayerHitbox : MonoBehaviour
{
    // Wird ausgelöst, wenn ein anderes Collider-Objekt in diesen Trigger eintritt
    private void OnTriggerEnter(Collider other)
    {
        // Wenn ein Geist den Spieler berührt → Game Over
        if (other.CompareTag("Ghost"))
        {
            GameManager.Instance.PlayerDied();
        }
    }
}
