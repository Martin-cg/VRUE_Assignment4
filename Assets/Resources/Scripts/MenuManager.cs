using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour {
    public InputAction ToggleMenuAction;
    public GameObject Menu;

    protected virtual void Start() {
        // SetMenuState(false);

        ToggleMenuAction.Enable();
        ToggleMenuAction.performed += ToggleMenuActionPerformed;
    }

    protected virtual void OnDestroy() {
        ToggleMenuAction.performed -= ToggleMenuActionPerformed;
    }

    private void ToggleMenuActionPerformed(InputAction.CallbackContext obj) => ToggleMenu();

    public void ToggleMenu() {
        SetMenuState(!GetMenuState());
    }

    public void SetMenuState(bool active) {
        Menu.SetActive(active);
    }
    public bool GetMenuState() {
        return Menu.activeSelf;
    }
}
