using UnityEngine;
using UnityEngine.InputSystem;

// Steuert die Animation der VR-Hand basierend auf Trigger- und Grip-Eingaben des Controllers.
public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty triggerValue;
    public InputActionProperty gripValue;

    // Animator der VR-Hand, besitzt die Parameter "Trigger" und "Grip"
    public Animator handAnimator;

    void Update()
    {
        // Controller-Eingaben auslesen
        float trigger = triggerValue.action.ReadValue<float>();
        float grip = gripValue.action.ReadValue<float>();

        // Animator-Werte setzen, um Hand zu animieren
        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);
    }
}
