using UnityEngine;

[DisallowMultipleComponent]
public class RoleSpecificBehaviour : RoleSpecific {
    public Behaviour[] Behaviours;

    protected override void OnRoleChanged(bool isRequiredRole) {
        foreach (var behaviour in Behaviours) {
            behaviour.enabled = isRequiredRole;
        }
    }
}
