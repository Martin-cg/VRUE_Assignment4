using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IngredientBox : MonoBehaviourGameStateCallbacks {
    public GameObject ItemPrefab;
    public IngredientInfo IngredientInfoPrefab;
    public Transform Socket;

    private GameObject currentItem;
    public GameObject CurrentItem {
        get => currentItem;
        set {
            currentItem = value;
            if (value) {
                CurrentItemRigidbody = CurrentItem.GetComponent<Rigidbody>();
                CurrentItemInteractable = CurrentItem.GetComponent<XRBaseInteractable>();
            } else {
                CurrentItemRigidbody = null;
                CurrentItemInteractable = null;
            }
        }
    }
    private Rigidbody CurrentItemRigidbody;
    private XRBaseInteractable CurrentItemInteractable;
    private bool CurrentItemReentered;

    protected virtual void Awake() {
        MasterClientOnly.AddTo(this);
    }

    protected virtual void Start() {
        Debug.Assert(ItemPrefab.GetComponent<TransformParentSync>());
        Debug.Assert(ItemPrefab.GetComponent<XRBaseInteractable>());
        Debug.Assert(ItemPrefab.GetComponent<Rigidbody>());
        Debug.Assert(ItemPrefab.GetComponent<TemporaryObject>());

        if (Socket.childCount != 0) {
            Debug.LogError($"Children in {nameof(IngredientBox)} socket are invalid and will be removed.", Socket);
            foreach (var child in Socket.GetChildren()) {
                Destroy(child);
            }
        }
    }

    public override void OnGameStarted() {
        base.OnGameStarted();

        GenerateItem();
    }

    public override void OnGameStopped() {
        base.OnGameStarted();

        if (CurrentItem) {
            PhotonNetwork.Destroy(CurrentItem);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (CurrentItem && other.attachedRigidbody?.gameObject == CurrentItem) {
            CurrentItemReentered = true;
        }
    }
    public void OnTriggerExit(Collider other) {
        if (CurrentItem && other.attachedRigidbody?.gameObject == CurrentItem) {
            CurrentItemReentered = false;
            // for some reason the item exits once when grabbed and then immediatly reenters for a few frames before exiting again...
            StartCoroutine(ItemMovedAwayReentrancyProtection());
        }
    }

    private IEnumerator ItemMovedAwayReentrancyProtection() {
        yield return null;
        yield return null;

        if (CurrentItem == null) {
            yield break;
        }

        if (!CurrentItemReentered) {
            OnItemMovedAway();
        }
    }

    private void GenerateItem() {
        // Debug.Log("GenerateItem() " + Socket.transform.position);
        var ingredientInfoPrefabPath = ResourcePathUtils.GetPrefabPath(IngredientInfoPrefab.gameObject);
        CurrentItem = PhotonNetwork.InstantiateRoomObject(ResourcePathUtils.GetPrefabPath(ItemPrefab), Socket.position, Socket.rotation, 0, new object[] { ingredientInfoPrefabPath });
        CurrentItem.transform.SetParent(Socket.transform, true);

        CurrentItemInteractable.selectEntered.AddListener(OnCurrentSelectEntered);
    }

    private void OnCurrentSelectEntered(SelectEnterEventArgs args) => OnCurrentItemGrabbed();

    private void OnCurrentItemGrabbed() {
        // Debug.Log("OnCurrentItemGrabbed()");
        CurrentItemInteractable.selectEntered.RemoveListener(OnCurrentSelectEntered);
        CurrentItemInteractable.selectExited.AddListener(OnTakenItemReleased);
    }

    private void OnTakenItemReleased(SelectExitEventArgs args) {
        // Debug.Log("OnTakenItemReleased()");
        var interactable = args.interactableObject;
        var obj = interactable.transform.gameObject;

        interactable.selectExited.RemoveListener(OnTakenItemReleased);
        obj.transform.SetParent(transform.parent, true);
    }

    private void OnItemMovedAway() {
        // Debug.Log("OnItemMovedAway()");

        CurrentItem.transform.SetParent(transform.parent, true);
        CurrentItem = null;
        GenerateItem();
    }
}
