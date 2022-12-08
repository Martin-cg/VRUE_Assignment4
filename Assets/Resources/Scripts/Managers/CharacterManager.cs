using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public sealed class CharacterManager : MonoBehaviourPunCallbacks {
    public static CharacterManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
    }

    public GameObject CharacterPrefab;
    public Transform Spawn;

    public UnityEvent<Character> CharacterSpawned = new();
    public UnityEvent<Character> LocalCharacterSpawned = new();

    private bool IsOfflineCharacter;
    private bool IsShuttingDown = false;

    private void Start() {
        SpawnLocalOfflineCharacter();
    }

    private void OnApplicationQuit() {
        IsShuttingDown = true;
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        SpawnLocalCharacter();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        SpawnLocalOfflineCharacter();
    }

    public void SpawnLocalCharacter() {
        if (IsShuttingDown) {
            return;
        }

        if (LocalCharacter.Instance && IsOfflineCharacter) {
            DestroyImmediate(LocalCharacter.Instance.gameObject);
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

        IsOfflineCharacter = false;
    }

    public void SpawnLocalOfflineCharacter() {
        if (IsShuttingDown) {
            return;
        }

        if (LocalCharacter.Instance && IsOfflineCharacter) {
            DestroyImmediate(LocalCharacter.Instance.gameObject);
        }

        var characterObject = Instantiate(CharacterPrefab, Spawn.position, Spawn.rotation);
        characterObject.name = "Local Offline Player";
        var character = characterObject.AddComponent<LocalCharacter>();
        character.Rig = GameObject.FindGameObjectWithTag(Tags.XROrigin).GetComponent<XRRig>();

        IsOfflineCharacter = true;
    }

}
