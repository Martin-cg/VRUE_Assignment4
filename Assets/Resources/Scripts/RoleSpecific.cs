using UnityEngine;

public abstract class RoleSpecific : MonoBehaviour {
    public CharacterRole RequiredRole;
    private Character Character;

    protected virtual void Start() {
        Character = GetComponent<Character>();
        if (Character == null) {
            Character = GetComponentInParent<Character>();
        }
        if (Character == null) {
            Character = Character.Local;
        }
        if (Character == null) {
            CharacterManager.Instance.CharacterSpawned.AddListener(OnLocalCharacterSpawned);
        } else {
            OnCharacterReady();
        }
    }
    protected virtual void OnDestroy() {
        if (Character != null && Character.isActiveAndEnabled) {
            Character.RoleChanged.RemoveListener(OnRoleChanged);
        }
        CharacterManager.Instance.LocalCharacterSpawned.RemoveListener(OnLocalCharacterSpawned);
    }

    private void OnLocalCharacterSpawned(Character character) {
        if (Character != null && Character.isActiveAndEnabled) {
            Character.RoleChanged.RemoveListener(OnRoleChanged);
        }

        Character = character;
        OnCharacterReady();
    }

    private void OnCharacterReady() {
        Character.RoleChanged.AddListener(OnRoleChanged);
        if (Character.Role != CharacterRole.Offline) {
            OnRoleChanged(Character.Role);
        }
    }

    protected abstract void OnRoleChanged(CharacterRole role);
}
