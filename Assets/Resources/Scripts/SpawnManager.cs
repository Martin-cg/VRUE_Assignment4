using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public GameObject PlayerSpawn;
    public GameObject SpectatorSpawn;

    public void SpawnAsPlayer() => Spawn(CharacterRole.Player);
    public void SpawnAsSpectator() => Spawn(CharacterRole.Spectator);
    public void Spawn(CharacterRole role) {
        var character = Character.Local;
        character.SetRole(role);

        Transform targetTransform = role switch {
            CharacterRole.Player => PlayerSpawn == null ? PlayerSpawn.transform : null,
            CharacterRole.Spectator => SpectatorSpawn == null ? SpectatorSpawn.transform : null,
            _ => null,
        };
        if (targetTransform != null ) {
            character.transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
        }
    }
}
