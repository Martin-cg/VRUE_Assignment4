using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRBaseInteractor))]
public class XRInteractorDebug : MonoBehaviour {
    private XRBaseInteractor Interactor;
    public GameObject Controller;
    public GameObject[] Selected;
    public GameObject[] Hovered;

    public void Start() {
        if (!Application.isEditor) {
            Destroy(this);
        }

        Interactor = GetComponent<XRBaseInteractor>();
    }

    public void Update() {
        if (!Interactor) {
            return;
        }

        Controller = (Interactor as XRBaseControllerInteractor)?.xrController?.gameObject;
        Selected = Interactor.interactablesSelected.Select(interactable => interactable.transform.gameObject).ToArray();
        Hovered = Interactor.interactablesHovered.Select(interactable => interactable.transform.gameObject).ToArray();
    }
}

