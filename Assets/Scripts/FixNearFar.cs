using System.Collections;
using UnityEngine;

public class FixNearFarInteractor : MonoBehaviour
{
    IEnumerator Start()
    {
        // Warten, bis die Physics Scene bereit ist
        yield return null;
        yield return null;

        gameObject.SetActive(false);
        yield return null;
        gameObject.SetActive(true);
    }
}
