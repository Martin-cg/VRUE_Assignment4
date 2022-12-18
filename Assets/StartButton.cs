using System.Collections;
using TMPro;
using UnityEngine;

public class StartButton : PredefinedActionButton {
    public GameManager GameManager;
    public int CountdownDuration;
    public TMP_Text Text;
    private Mode CurrentMode;

    private enum Mode {
        Start,
        Stop,
        Noop
    }

    protected override void OnButtonClicked() {
        switch (CurrentMode) {
            case Mode.Start:
                CurrentMode = Mode.Noop;
                StartCoroutine(Countdown());
                break;
            case Mode.Stop:
                CurrentMode = Mode.Start;
                Text.text = "Start";
                GameManager.ResetGame();
                break;
        }

    }

    private IEnumerator Countdown() {
        var current = CountdownDuration;
        while (current > 0) {
            Text.text = current.ToString();
            yield return new WaitForSeconds(1);
            current--;
        }
        Text.text = "GO";
        GameManager.StartGame();

        yield return new WaitForSeconds(10);
        CurrentMode = Mode.Noop;
        Text.text = "Reset";
    }
}
