using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonController : SynchronizedRoomObject {
    public XRBaseInteractable Interactable;
    public UnityEvent<bool> StateChanged = new();

    [SerializeField]
    private bool isPressed = false;
    public bool IsPressed {
        get => isPressed;
        set {
            var changed = isPressed != value;
            isPressed = value;
            if (changed) {
                OnStateChanged();
            }
        }
    }
    public bool IsSwitch = true;
    public float DebounceTime = 0.2f;
    private float LastPressedTime;

    protected override void Start() {
        base.Start();

        Init();
        RegisterProperty<bool>(nameof(IsPressed), newValue => IsPressed = newValue);
    }

    private void Reset() {
        Init();
    }

    private void Init() {
        Interactable = Interactable == null ? GetComponent<XRBaseInteractable>() : Interactable;
        Interactable = Interactable == null ? GetComponentInChildren<XRBaseInteractable>() : Interactable;
    }

    protected virtual void OnStateChanged() {
        StateChanged?.Invoke(isPressed);

        SetProperty(nameof(IsPressed), IsPressed, false);
    }

    protected virtual void OnPressed() {
        if (Time.time - LastPressedTime <= DebounceTime) {
            return;
        }
        LastPressedTime = Time.time;

        if (IsSwitch) {
            IsPressed = !IsPressed;
        } else {
            IsPressed = true;
        }
    }

    protected virtual void OnReleased() {
        if (IsSwitch) {
        } else {
            IsPressed = false;
        }
    }
}
