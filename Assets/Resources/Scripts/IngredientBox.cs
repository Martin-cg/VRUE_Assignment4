using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IngredientBox : MonoBehaviourPun {
    public GameObject ItemPrefab;
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
    private GameObject LastItem;
    private bool LastItemReentered;

    protected virtual void Reset() {
        GetComponent<MasterClientOnly>().TargetBehaviours.Add(this);
    }

    protected virtual void Start() {
        Debug.Assert(ItemPrefab.GetComponent<TransformParentSync>());
        Debug.Assert(ItemPrefab.GetComponent<XRBaseInteractable>());
        Debug.Assert(ItemPrefab.GetComponent<Rigidbody>());

        if (Socket.childCount != 0) {
            Debug.LogError($"Children in {nameof(IngredientBox)} socket are invalid and will be removed.", Socket);
            foreach (var child in Socket.GetChildren()) {
                Destroy(child);
            }
        }
        
        GenerateItem();
    }

    public void OnTriggerEnter(Collider other) {
        if (LastItem && other.attachedRigidbody?.gameObject == LastItem) {
            LastItemReentered = true;
        }
    }
    public void OnTriggerExit(Collider other) {
        if (LastItem && other.attachedRigidbody?.gameObject == LastItem) {
            LastItemReentered = false;
            // for some reason the item exits once when grabbed and then immediatly reenters for a few frames before exiting again...
            StartCoroutine(ItemMovedAwayReentrancyProtection());
        }
    }

    private IEnumerator ItemMovedAwayReentrancyProtection() {
        yield return null;
        yield return null;

        if (!LastItemReentered) {
            OnItemMovedAway();
        }
    }

    private void GenerateItem() {
        Debug.Log("GenerateItem() " + Socket.transform.position);
        CurrentItem = PhotonNetwork.InstantiateRoomObject($"Prefabs/{ItemPrefab.name}", Socket.position, Socket.rotation);
        CurrentItem.transform.SetParent(Socket.transform, true);

        CurrentItemInteractable.selectEntered.AddListener(OnCurrentSelectEntered);
    }

    private void OnCurrentSelectEntered(SelectEnterEventArgs args) => OnCurrentItemGrabbed();

    private void OnCurrentItemGrabbed() {
        Debug.Log("OnCurrentItemGrabbed()");
        CurrentItemInteractable.selectEntered.RemoveListener(OnCurrentSelectEntered);
        CurrentItemInteractable.selectExited.AddListener(OnTakenItemReleased);
        CurrentItem.transform.SetParent(transform.parent, true);
        (CurrentItem, LastItem) = (null, CurrentItem);
    }

    private static void OnTakenItemReleased(SelectExitEventArgs args) {
        Debug.Log("OnTakenItemReleased()");
        var interactable = args.interactableObject;
        var obj = interactable.transform.gameObject;

        interactable.selectExited.RemoveListener(OnTakenItemReleased);
        
        if (obj.GetComponent<Rigidbody>() is var rigidbody) {
            // rigidbody.useGravity = true;
            // rigidbody.isKinematic = false;
        }
    }

    private void OnItemMovedAway() {
        Debug.Log("OnItemMovedAway()");
        Debug.Assert(CurrentItem == null);

        LastItem = null;
        GenerateItem();
    }
}
