using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TeleportActivator : MonoBehaviour
{

    public InputActionReference TeleportActivate;

    public UnityEvent OnTeleportActivate;
    public UnityEvent OnTeleportDeactivate;

    // Start is called before the first frame update
    void Start()
    {
        TeleportActivate.action.performed += ActivateTeleport;
        TeleportActivate.action.canceled += DeactivateTeleport;
    }

    private void DeactivateTeleport(InputAction.CallbackContext obj) {
        OnTeleportDeactivate.Invoke();
    }

    private void ActivateTeleport(InputAction.CallbackContext obj) {
        OnTeleportActivate.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
