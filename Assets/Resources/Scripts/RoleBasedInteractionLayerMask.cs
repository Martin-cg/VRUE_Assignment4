using UnityEngine.XR.Interaction.Toolkit;

public class RoleBasedInteractionLayerMask : RoleSpecific {
    public XRBaseInteractor Interactor;
    public XRBaseInteractable Interactable;

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
