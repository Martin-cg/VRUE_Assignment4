using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoleBasedInteractionLayerMask : RoleSpecific {
    public Component Target;

    private XRBaseInteractor Interactor;
    private XRBaseInteractable Interactable;

    protected virtual void OnEnable() {
        Interactor = Target.GetComponent<XRBaseInteractor>();
        Interactable = Target.GetComponent<XRBaseInteractable>();
        Target = Interactor == null ? Interactable : Interactor;
    }

    protected override void OnRoleChanged(CharacterRole role) {
        var layer = role switch {
            CharacterRole.Player => InteractionLayers.Player,
            CharacterRole.Spectator => InteractionLayers.Spectator,
            _ => 0
        };
        var mask = new InteractionLayerMask() { value = layer };

        if (Interactor) {
            Interactor.interactionLayers = mask;
        }

        if (Interactable) {
            Interactable.interactionLayers = mask;
        }
    }
}
