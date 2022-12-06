public class RoleSpecificObject : RoleSpecific {
    protected override void OnRoleChanged(CharacterRole role) {
        gameObject.SetActive(role == RequiredRole);
    }
}
