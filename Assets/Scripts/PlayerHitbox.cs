using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            GameManagerVR.Instance.PlayerDied();
        }
    }
}
