using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PanGrip : XRGrabInteractable {
    public Transform StoveAttachTransform;

    public override Transform GetAttachTransform(IXRInteractor interactor) {
        if (interactor is StoveSocket) {
            return StoveAttachTransform;
        } else {
            return base.GetAttachTransform(interactor);
        }
    }
}
