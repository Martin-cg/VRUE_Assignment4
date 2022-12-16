public class TemporaryObject : MonoBehaviourGameStateCallbacks {
    public override void OnGameStopped() {
        base.OnGameStopped();

        Destroy(gameObject);
    }
}
