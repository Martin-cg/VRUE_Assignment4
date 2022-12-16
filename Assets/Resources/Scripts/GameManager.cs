using System;
using System.Linq;
using UnityEngine;

public class GameManager : SynchronizedRoomObject {
    private static GameManager _Instance;
    public static GameManager Instance => _Instance == null ? _Instance = FindObjectOfType<GameManager>() : _Instance;

    protected override void Awake() {
        base.Awake();

        if (Instance != null && Instance != this) {
            if (gameObject.GetComponentsInChildren<Component>().All(component => component is not Transform || component == this)) {
                Destroy(this.gameObject);
            } else {
                Destroy(this);
            }
        } else {
            _Instance = this;
        }
    }

    public event Action<GameState> OnGameStateChanged;
    public GameState State { get; private set; }

    public void StartGame() {
        if (State == GameState.InProgress) {
            return;
        }

        State = GameState.InProgress;
        OnGameStateChanged?.Invoke(State);
    }

    public void ResetGame() {
        if (State == GameState.Preparing) {
            return;
        }

        State = GameState.Preparing;
        OnGameStateChanged?.Invoke(State);
    }

    public void AddCallbackTarget(IGameStateCallbacks target) {
        OnGameStateChanged += target.OnGameStateChanged;
    }

    public void RemoveCallbackTarget(IGameStateCallbacks target) {
        OnGameStateChanged -= target.OnGameStateChanged;
    }
}
