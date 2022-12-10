using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoleBasedInteractionLayerMask : RoleDependent {
    public Component Target;

    private XRBaseInteractor Interactor;
    private XRBaseInteractable Interactable;

    protected virtual void OnValidate() {
        SetInteractionLayers(0);
    }

    protected virtual void OnEnable() {
        Interactor = Target.GetComponent<XRBaseInteractor>();
        Interactable = Target.GetComponent<XRBaseInteractable>();
        Target = Interactor == null ? Interactable : Interactor;
        SetInteractionLayers(0);
    }

    protected override void OnRoleChanged(CharacterRole role) {
        var layer = role switch {
            CharacterRole.Player => InteractionLayers.Player,
            CharacterRole.Spectator => InteractionLayers.Spectator,
            CharacterRole.Offline => InteractionLayers.Offline,
            _ => 0
        };
        var mask = new InteractionLayerMask() { value = layer };
        SetInteractionLayers(mask);
    }

    private void SetInteractionLayers(InteractionLayerMask mask) {
        if (Interactor) {
            Interactor.interactionLayers = mask;
        }

        if (Interactable) {
            Interactable.interactionLayers = mask;
        }
    }
}
