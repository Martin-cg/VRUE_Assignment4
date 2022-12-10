using UnityEngine;

[DisallowMultipleComponent]
public class RoleSpecificBehaviour : RoleSpecific {
    public Behaviour[] Behaviours;

    protected override void OnRoleChanged(CharacterRole role) {
        foreach (var behaviour in Behaviours) {
            behaviour.enabled = role == RequiredRole;
        }
    }
}
