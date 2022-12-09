using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SynchronizedSocketInteractor))]
public class SynchronizedSocketInteractorTest : MonoBehaviour {
    public SynchronizedSocketInteractor Interactor;

    public void Start() {
        Interactor = GetComponent<SynchronizedSocketInteractor>();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(SynchronizedSocketInteractorTest))]
public class SynchronizedSocketInteractorTestEditor : Editor {
    private string IdString = "";

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = this.target as SynchronizedSocketInteractorTest;

        IdString = GUILayout.TextField(IdString);
        if (GUILayout.Button("Change")) {
            var id = int.TryParse(IdString, out int result) ? (int?)result : null;
            var socket = target.GetComponent<XRSocketInteractor>();
            if (id == null) {
                socket.interactionManager.SelectExit(socket, socket.firstInteractableSelected);
            } else {
                var interactable = (IXRSelectInteractable) PhotonView.Find(id.Value).GetComponent<XRBaseInteractable>();
                socket.interactionManager.SelectEnter(socket, interactable);
            }
        }
    }
}
#endif
