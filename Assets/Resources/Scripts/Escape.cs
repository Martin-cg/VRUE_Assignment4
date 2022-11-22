using UnityEngine;

public class Escape : Singleton<Escape> {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            Application.Quit();
        }
    }
}
