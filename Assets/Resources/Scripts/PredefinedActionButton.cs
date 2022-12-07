using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class PredefinedActionButton : MonoBehaviour {
    public Button Button;

    protected virtual void Reset() {
        FindButton(true);
    }

    protected virtual void OnValidate() {
        FindButton(true);
    }

    protected virtual void Awake() {
        FindButton(false);

        Button.onClick.AddListener(OnButtonClicked);
    }

    protected virtual void OnDestroy() {
        if (Button) {
            Button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void FindButton(bool ignoreCurrent) {
        if (ignoreCurrent || Button == null) {
            Button = GetComponent<Button>();
        }
    }

    protected abstract void OnButtonClicked();

}
