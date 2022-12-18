using UnityEngine.XR.Interaction.Toolkit;

public class StoveSocket : XRSocketInteractor {
    private bool IsPan(IXRInteractable interactable) => interactable.transform.gameObject.CompareTag(Tags.Pan);
    public override bool CanHover(IXRHoverInteractable interactable) => base.CanHover(interactable) && IsPan(interactable);
    public override bool CanSelect(IXRSelectInteractable interactable) => base.CanSelect(interactable) && IsPan(interactable);
}
