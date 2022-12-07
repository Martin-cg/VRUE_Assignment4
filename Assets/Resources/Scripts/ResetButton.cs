public class ResetButton : PredefinedActionButton {
    public GameManager GameManager;

    protected override void OnButtonClicked() {
        GameManager.ResetGame();
    }
}
