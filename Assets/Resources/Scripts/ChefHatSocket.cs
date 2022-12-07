using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

public class ChefHatSocket : XRSocketInteractor {
    private LocalCharacter Character;
    private HashSet<IXRSelectInteractable> PotentialTargets = new();

    protected override void Reset() {
        base.Reset();

        FindReferences();
    }

    protected override void Start() {
        base.Awake();

        FindReferences();
    }

    private void FindReferences() {
        Character = Character == null ? GetComponentInParent<LocalCharacter>() : Character;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args) {
        base.OnHoverEntered(args);

        var interactable = args.interactableObject as IXRSelectInteractable;
        if (interactable.transform.HasComponent<ChefHat>()
            && interactable.firstInteractorSelecting != null
            && Character.Rig.transform.IsParentOf(interactable.firstInteractorSelecting.transform)) {
            PotentialTargets.Add(interactable);
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args) {
        base.OnHoverExited(args);

        var interactable = args.interactableObject as IXRSelectInteractable;
        PotentialTargets.Remove(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable) {
        return IsSelecting(interactable) || (interactablesSelected.Count == 0 && PotentialTargets.Contains(interactable));
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        Character.Character.SetRole(CharacterRole.Player);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        Character.Character.SetRole(CharacterRole.Spectator);
    }
}
