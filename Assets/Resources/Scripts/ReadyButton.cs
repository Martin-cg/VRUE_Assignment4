using TMPro;
using UnityEngine;

[RequireComponent(typeof(ButtonController))]
public class ReadyButton : MonoBehaviour {
    public ButtonController Button;
    public TMP_Text Text;
    public Renderer ButtonRenderer;
    public Color NormalColor;
    public Color ReadyColor;

    private void Start() {
        Button = GetComponent<ButtonController>();
        Button.IsSwitch = true;
        Button.IsPressed = false;
        ButtonRenderer = GetComponent<Renderer>();
        Button.StateChanged.AddListener(pressed => UpdateButton());
        UpdateButton();
    }

    private void UpdateButton() {
        Text.text = Button.IsPressed ? "Ready" : "Not Ready";
        ButtonRenderer.material.color = Button.IsPressed ? ReadyColor : NormalColor;
    }
}
