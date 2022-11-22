using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XROrigin))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterControllerDriver))]
public class CharacterControllerCameraConstrainer : MonoBehaviour {

    public XROrigin XROrigin;
    public CharacterController CharacterController;
    public CharacterControllerDriver CharacterControllerDriver;

    // Start is called before the first frame update
    void Start() {
        XROrigin = GetComponent<XROrigin>();
        CharacterController = GetComponent<CharacterController>();
        CharacterControllerDriver = GetComponent<CharacterControllerDriver>();
    }

    // Update is called once per frame
    void Update() {
        UpdateCharacterController();
    }

    /// <summary>
    /// Modified to update every Update()
    /// Updates the <see cref="CharacterController.height"/> and <see cref="CharacterController.center"/>
    /// based on the camera's position.
    /// </summary>
    protected virtual void UpdateCharacterController() {
        if (XROrigin == null || CharacterController == null)
            return;

        var height = Mathf.Clamp(XROrigin.CameraInOriginSpaceHeight, CharacterControllerDriver.minHeight, CharacterControllerDriver.maxHeight);

        Vector3 center = XROrigin.CameraInOriginSpacePos;
        center.y = height / 2f + CharacterController.skinWidth;

        CharacterController.height = height;
        CharacterController.center = center;
    }
}
