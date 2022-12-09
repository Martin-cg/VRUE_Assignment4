using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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

        if (string.IsNullOrWhiteSpace(PhotonNetwork.LocalPlayer.NickName)) {
            PhotonNetwork.LocalPlayer.NickName = $"Player {PhotonNetwork.LocalPlayer.ActorNumber}";
        }

        SpawnLocalCharacter();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        SpawnLocalOfflineCharacter();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.TryGetValue(nameof(Character.Role), out var newRole)) {
            Character.ForPlayer(targetPlayer).SetRole((CharacterRole) newRole);
        }
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
