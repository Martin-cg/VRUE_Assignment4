using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public sealed class CharacterManager : Singleton<CharacterManager> {
    public GameObject CharacterPrefab;
    public Transform Spawn;

    public UnityEvent<Character> CharacterSpawned = new();
    public UnityEvent<Character> LocalCharacterSpawned = new();
    private GameObject LocalOfflineCharacter;

    public void Start() {
        // Create a new Character instance so that the user can see his hands and stuff until we connect.
        SpawnLocalOfflineCharacter();
    }

    public void SpawnLocalCharacter() {
        if (LocalOfflineCharacter) {
            DestroyImmediate(LocalOfflineCharacter);
        }

        var characterObject = PhotonNetwork.Instantiate($"Prefabs/{CharacterPrefab.name}", Spawn.position, Spawn.rotation, default, new object[] {
            new Character.InstantiationData() { ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber }
        });
        characterObject.name = "Local Player";
        var character = characterObject.GetComponent<Character>();
        var localCharacter = characterObject.AddComponent<LocalCharacter>();
        localCharacter.Rig = GameObject.FindGameObjectWithTag(Tags.XROrigin).GetComponent<XRRig>();

        CharacterSpawned.Invoke(character);
        LocalCharacterSpawned.Invoke(character);
    }

    public void SpawnLocalOfflineCharacter() {
        LocalOfflineCharacter = Instantiate(CharacterPrefab, Spawn.position, Spawn.rotation);
        LocalOfflineCharacter.name = "Local Offline Player";
        var character = LocalOfflineCharacter.AddComponent<LocalCharacter>();
        character.Rig = GameObject.FindGameObjectWithTag(Tags.XROrigin).GetComponent<XRRig>();
    }

}
