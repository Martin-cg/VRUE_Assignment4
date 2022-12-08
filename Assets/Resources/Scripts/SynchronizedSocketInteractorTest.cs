using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SynchronizedSocketInteractor))]
public class SynchronizedSocketInteractorTest : MonoBehaviour {
    public SynchronizedSocketInteractor Interactor;
    public GameObject Current;

    public void Update() {
        Current = Interactor.GetComponent<XRSocketInteractor>().firstInteractableSelected.transform.gameObject;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SynchronizedSocketInteractorTest))]
public class SynchronizedSocketInteractorTestEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = this.target as SynchronizedSocketInteractorTest;

        var idString = GUILayout.TextField("");
        if (GUILayout.Button("Change")) {
            var id = int.TryParse(idString, out int result) ? (int?)result : null;
            target.Interactor.SetProperty<int?>("CurrentInteractableSceneViewId", id);
        }
    }
}
#endif
