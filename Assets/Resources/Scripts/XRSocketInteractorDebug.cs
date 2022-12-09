using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class XRSocketInteractorDebug : MonoBehaviour {
    private XRSocketInteractor Interactor;
    public GameObject Current;

    public void Start() {
        Interactor = GetComponent<XRSocketInteractor>();
    }

    public void Update() {
        Current = Interactor?.firstInteractableSelected?.transform?.gameObject;
    }
}

