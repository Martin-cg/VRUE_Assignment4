using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameManager : SynchronizedRoomObject {
    private static GameManager _Instance;
    public static GameManager Instance => _Instance == null ? _Instance = FindObjectOfType<GameManager>() : _Instance;

    public bool AutoStart = true;

    protected override void Awake() {
        base.Awake();

        if (Instance != null && Instance != this) {
            if (gameObject.GetComponentsInChildren<Component>().All(component => component is not Transform || component == this)) {
                Destroy(gameObject);
            } else {
                Destroy(this);
            }
        } else {
            _Instance = this;
        }
    }

    protected override void Start() {
        base.Start();

        if (AutoStart) {
            StartGame();
        }
    }

    public event Action<GameState> OnGameStateChanged;
    public GameState _State = GameState.Preparing;
    public GameState State {
        get => _State;
        set {
            if (_State == value) {
                return;
            }

            _State = value;
            OnGameStateChanged?.Invoke(State);
        }
    }

    public void StartGame() => State = GameState.InProgress;

    public void ResetGame() => State = GameState.Preparing;

    public void AddCallbackTarget(IGameStateCallbacks target) {
        OnGameStateChanged += target.OnGameStateChanged;
    }

    public void RemoveCallbackTarget(IGameStateCallbacks target) {
        OnGameStateChanged -= target.OnGameStateChanged;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects] // workaround because "GameManager" is somehow special and cannot have a custom editor otherwise
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = this.target as GameManager;


        GUI.enabled = target.State == GameState.Preparing;
        if (GUILayout.Button("Start")) {
            target.State = GameState.InProgress;
        }

        GUI.enabled = target.State == GameState.InProgress;
        if (GUILayout.Button("Stop")) {
            target.State = GameState.Preparing;
        }
    }
}
#endif
