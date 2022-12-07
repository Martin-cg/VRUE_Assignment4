using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Undroppable : MonoBehaviour {
    private XRGrabInteractable Interactable;
    private XRInteractionManager InteractionManager => Interactable.interactionManager;
    private XRSocketInteractor LastSocket;
    public DropMode Mode;

    protected virtual void Reset() => Init();
    protected virtual void Awake() => Init();
    protected virtual void Start() {
        if (!Interactable.isSelected) {
            Debug.LogError($"{nameof(Undroppable)} is not selected on startup.", this);
        }
    }

    private void Init() {
        Interactable = Interactable == null ? GetComponent<XRGrabInteractable>() : Interactable;
    }

    protected virtual void OnEnable() {
        Interactable.selectExited.AddListener(OnSelectExit);
    }
    protected virtual void OnDisable() {
        Interactable.selectExited.RemoveListener(OnSelectExit);
    }

    private void OnSelectExit(SelectExitEventArgs args) {
        if (args.interactorObject is XRSocketInteractor socketInteractor) {
            LastSocket = socketInteractor;
        }

        if (isActiveAndEnabled && gameObject.activeInHierarchy) {
            StartCoroutine(AntiDrop());
        }
    }

    private IEnumerator AntiDrop() {
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        if (Interactable.isSelected) {
            yield break;
        }
        switch (Mode) {
            case DropMode.Destroy:
                Destroy(gameObject);
                break;
            case DropMode.Return:
                if (InteractionManager && LastSocket && Interactable) {
                    InteractionManager.SelectEnter(LastSocket, (IXRSelectInteractable)Interactable);
                }
                break;
        }
    }

    public enum DropMode {
        Destroy,
        Return
    }
}
