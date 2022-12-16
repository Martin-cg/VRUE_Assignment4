using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class MenuManager : MonoBehaviour {
    public InputAction ToggleMenuAction;
    public GameObject Menu;
    public XRRig Rig;
    public PostProcessLayer BlurLayer;
    public float DistanceToHead = 3.0F;
    public float SmoothTime = 0.3F;
    private Vector3 Velocity = Vector3.zero;

    protected virtual void Start() {
        SetMenuState(false);

        ToggleMenuAction.Enable();
        ToggleMenuAction.performed += ToggleMenuActionPerformed;
    }

    protected virtual void Update() {
        if (GetMenuState()) {
            UpdateMenuPosition(true);
        }
    }

    private void UpdateMenuPosition(bool smooth) {
        var targetPosition = Rig.Head.transform.TransformPoint(new Vector3(0, 0, DistanceToHead));
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref Velocity, smooth ? SmoothTime : 0);
        transform.LookAt(Rig.Head.transform.position);
    }

    protected virtual void OnDestroy() {
        ToggleMenuAction.performed -= ToggleMenuActionPerformed;
    }

    private void ToggleMenuActionPerformed(InputAction.CallbackContext obj) => ToggleMenu();

    public void ToggleMenu() => SetMenuState(!GetMenuState());

    public void SetMenuState(bool active) {
        if (active) {
            UpdateMenuPosition(false);
        }

        BlurLayer.enabled = active;

        var objects = FindObjectsOfType<RenderInFrontOfMenu>();
        foreach (var obj in objects) {
            obj.gameObject.layer = active ? Layers.AlwaysInFront : Layers.Default;
        }

        Menu.SetActive(active);
    }
    public bool GetMenuState() => Menu.activeSelf;
}
