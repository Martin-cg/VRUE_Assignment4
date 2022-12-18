using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StoveSocket : XRSocketInteractor {
    public ParticleSystem[] Fire;
    // private PanSocket Pan;

    private bool IsPan(IXRInteractable interactable) => interactable.transform.gameObject.CompareTag(Tags.Pan);
    public override bool CanHover(IXRHoverInteractable interactable) => base.CanHover(interactable) && IsPan(interactable);
    public override bool CanSelect(IXRSelectInteractable interactable) => base.CanSelect(interactable) && IsPan(interactable);

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        // Pan = args.interactableObject.transform.gameObject.GetComponentInChildren<PanSocket>();

        foreach (var system in Fire) {
            system.Play();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        // Pan = args.interactableObject.transform.gameObject.GetComponentInChildren<PanSocket>();

        foreach (var system in Fire) {
            system.Stop();
        }
    }
}
