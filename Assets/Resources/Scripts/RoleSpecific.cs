using UnityEngine;

public abstract class RoleSpecific : MonoBehaviour {
    public CharacterRole RequiredRole;
    private Character Character;

    protected virtual void Start() {
        if (Character.Local != null) {
            OnLocalCharacterSpawned(Character.Local);
        } else {
            CharacterManager.Instance.LocalCharacterSpawned.AddListener(OnLocalCharacterSpawned);
        }
    }
    protected virtual void OnDestroy() {
        if (Character != null && Character.isActiveAndEnabled) {
            Character.RoleChanged.RemoveListener(OnRoleChanged);
        }
    }

    private void OnLocalCharacterSpawned(Character character) {
        Character = character;
        Character.RoleChanged.AddListener(OnRoleChanged);
        CharacterManager.Instance.LocalCharacterSpawned.RemoveListener(OnLocalCharacterSpawned);

        if (Character.Role != CharacterRole.Offline) {
            OnRoleChanged(Character.Role);
        }
    }

    protected abstract void OnRoleChanged(CharacterRole role);
}
