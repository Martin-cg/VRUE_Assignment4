using UnityEngine;

[DisallowMultipleComponent]
public class RoleSpecificObject : RoleSpecific {
    protected override void OnRoleChanged(bool isRequiredRole) {
        gameObject.SetActive(isRequiredRole);
    }
}
