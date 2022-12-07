using UnityEngine.XR.Interaction.Toolkit;

public class ChefHatSocket : XRSocketInteractor {
    private Character Character;

    protected override void Reset() {
        base.Reset();

        FindReferences();
    }

    protected override void Awake() {
        base.Awake();

        FindReferences();
    }

    private void FindReferences() {
        Character = Character == null ? GetComponentInParent<Character>() : Character;
    }

    public override bool CanHover(IXRHoverInteractable interactable) {
        return base.CanHover(interactable)
            && interactable.transform.HasComponent<ChefHat>()
            && interactable is IXRSelectInteractable selectInteractable
            && Character.transform.IsParentOf(selectInteractable.GetOldestInteractorSelecting().transform);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        Character.SetRole(CharacterRole.Player);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        Character.SetRole(CharacterRole.Spectator);
    }
}
