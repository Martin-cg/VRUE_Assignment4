using UnityEngine;

public class ConsoleToGui : MonoBehaviour {
    private static string logText = "";
    private string output;

    private void Awake() {
        // Do not display in editor
        if (Application.isEditor) {
            Destroy(this);
        }
    }

    private void OnEnable() {
        Application.logMessageReceived += Log;
    }

    private void OnDisable() {
        Application.logMessageReceived -= Log;
    }

    private void Log(string logString, string stackTrace, LogType type) {
        output = logString;
        logText = output + "\n" + logText;
        if (logText.Length > 5000) {
            logText = logText.Substring(0, 4000);
        }
    }

    private void OnGUI() {
        logText = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), logText);
    }
}
