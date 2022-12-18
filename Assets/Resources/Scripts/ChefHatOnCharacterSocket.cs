﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ChefHatOnCharacterSocket : XRSocketInteractor {
    private LocalCharacter Character;

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

    public override void GetValidTargets(List<IXRInteractable> targets) {
        base.GetValidTargets(targets);

        for (int i=targets.Count-1; i>=0; i--) {
            var interactable = targets[i] as IXRHoverInteractable;
            if (!interactable.transform.HasComponent<ChefHat>() || !BelongsToLocalCharacter(interactable)) {
                targets.RemoveAt(i);
            }
        }
    }

    private bool BelongsToLocalCharacter(IXRHoverInteractable interactable) {
        return Character && Character.Rig && interactable.interactorsHovering.Any(interactor => !ReferenceEquals(interactor, this) && Character.Rig.transform.IsParentOf(interactor.transform));
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        UpdateRole();
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        UpdateRole();
    }

    protected virtual void Update() {
        UpdateRole();
    }

    private void UpdateRole() {
        var role = hasSelection ? CharacterRole.Player : CharacterRole.Spectator;
        Character.Character.SetRole(role);
    }
}
