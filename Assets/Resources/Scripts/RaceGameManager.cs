using Photon.Pun;
using TMPro;
using UnityEngine;

public class RaceGameManager : MonoBehaviourPun, IPunObservable {
    private enum GameState {
        PreGame,
        Countdown,
        InGame,
        PostGame
    };

    public ButtonController ResetButton;
    public LaneManager LaneManager;
    public TextMeshPro CountdownText;

    private GameState currentState = GameState.PreGame;
    private GameState CurrentState {
        get => currentState;
        set {
            if (currentState == value) { 
                return;
            }
            currentState = value;

            SetRaceBarrierState(CurrentState == GameState.PreGame || CurrentState == GameState.Countdown);
        }
    }

    // STATE PREGAME
    private int ReadyLanes = 0;

    // STATE COUNTDOWN
    float? Countdown;

    // Start is called before the first frame update
    void Start() {
        foreach (Lane l in LaneManager.Lanes) {
            l.ReadyButton.Button.StateChanged.AddListener(OnLaneStateChange);
        }
        ResetButton.StateChanged.AddListener(OnReset);
    }

    // Update is called once per frame
    void Update() {
        switch(CurrentState) {
            case GameState.PreGame:
                HandlePreGameUpdate();
                break;
            case GameState.Countdown:
                HandleCountdownUpdate();
                break;
            case GameState.InGame:
                HandleInGameUpdate();
                break;
            case GameState.PostGame:
                HandlePostGameUpdate();
                break;
        }
    }

    public void OnLaneStateChange(bool active) {
        if (active) {
            ReadyLanes++;
        } else {
            ReadyLanes--;
        }
    }

    public void OnReset(bool active) {
        if (!PhotonNetwork.IsMasterClient || !active || CurrentState != GameState.PostGame) {
            return;
        }

        SetReadyButtonState(false);

        CurrentState = GameState.PreGame;
        CountdownText.SetText("");
        photonView.RPC(nameof(ResetPlayer), RpcTarget.All);
        LaneManager.Setup();
    }

    public void OnGoalCrossed(int laneNumber) {
        if (CurrentState == GameState.InGame && PhotonNetwork.IsMasterClient) {
			CountdownText.SetText($"{(laneNumber == 0 ? "Left" : "Right")} won!");
            CurrentState = GameState.PostGame;
        }
    }

    private void HandlePreGameUpdate() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        CountdownText.fontSize = 48;
        CountdownText.SetText($"{ReadyLanes}/2 ready");

        if (ReadyLanes == 2) {
            CurrentState = GameState.Countdown;
            Countdown = 5.0F;
        }
    }
    
    private void HandleCountdownUpdate() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        if (ReadyLanes != 2) {
            CurrentState = GameState.PreGame;
            return;
        }

        if (!Countdown.HasValue) {
            Countdown = 5.0F;
        }

        Countdown -= Time.deltaTime;
        if (Countdown <= 0.0F) {
            Countdown = 0.0F;
            CurrentState = GameState.InGame;
            CountdownText.fontSize = 48;
            CountdownText.SetText("GO");
            SetReadyButtonState(false);
            return;
        }

        CountdownText.fontSize = 26;
        CountdownText.SetText($"Game starting in\n{Countdown:F1}");
    }
    
    private void HandleInGameUpdate() {
    }

    private void HandlePostGameUpdate() {
    }

    [PunRPC]
    private void ResetPlayer() {
        Character.Local.SpawnAtLane(Character.Local.AssignedLane);
    }

    private void SetReadyButtonState(bool active) {
        foreach (Lane l in LaneManager.Lanes) {
            l.ReadyButton.Button.IsPressed = active;
        }
    }
    private void SetRaceBarrierState(bool active) {
        foreach (Lane l in LaneManager.Lanes) {
            l.RaceBarrier.SetActive(active);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsReading) {
            CurrentState = (GameState)stream.ReceiveNext();
        } else {
            stream.SendNext(CurrentState);
        }
    }
}
