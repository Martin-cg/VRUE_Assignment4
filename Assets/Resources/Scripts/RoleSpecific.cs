public abstract class RoleSpecific : RoleDependent {
    public CharacterRole RequiredRole;

    protected override void OnRoleChanged(CharacterRole role) {
        OnRoleChanged(role == RequiredRole);
    }

    protected abstract void OnRoleChanged(bool isRequiredRole);
}
