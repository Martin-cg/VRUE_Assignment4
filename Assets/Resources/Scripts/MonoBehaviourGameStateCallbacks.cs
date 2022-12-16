using UnityEngine;

public class MonoBehaviourGameStateCallbacks : MonoBehaviour, IGameStateCallbacks {
    protected virtual void OnEnable() {
        GameManager.Instance.AddCallbackTarget(this);
    }

    protected virtual void OnDisable() {
        GameManager.Instance.RemoveCallbackTarget(this);
    }

    public virtual void OnGameStateChanged(GameState newState) {
        switch (newState) {
            case GameState.Preparing: OnGameStopped(); break;
            case GameState.InProgress: OnGameStarted(); break;
        }
    }

    public virtual void OnGameStarted() {}

    public virtual void OnGameStopped() {}
}
