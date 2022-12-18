using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class DisallowGrab : MonoBehaviour, IXRSelectFilter, IXRHoverFilter {
    public XRGrabInteractable Interactable;

    public bool canProcess => true;

    private void OnCollisionEnter(Collision collision) {
        Debug.LogWarning(collision.collider);
    }

    private bool CanInteract(IXRInteractable interactable) => interactable is XRDirectInteractor d;
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) => CanInteract(interactable);
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable) => CanInteract(interactable);

    protected virtual void OnEnable() {
        if (Interactable) {
            Interactable.selectFilters.Add(this);
            Interactable.hoverFilters.Add(this);
        }
    }

    protected virtual void OnDisable() {
        if (Interactable) {
            Interactable.selectFilters.Remove(this);
            Interactable.hoverFilters.Remove(this);
        }
    }
}
