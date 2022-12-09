using Photon.Pun;
using System.Linq;
using UnityEngine;

public class IngredientBox : MonoBehaviourPun {
    public GameObject ItemPrefab;
    public Transform Socket;
    public Collider RefillRegion;

    private GameObject currentItem;
    public GameObject CurrentItem {
        get => currentItem;
        set {
            currentItem = value;
            if (value) {
                CurrentItemRigidbody = CurrentItem.GetComponent<Rigidbody>();
            } else {
                CurrentItemRigidbody = null;
            }
        }
    }
    private Rigidbody CurrentItemRigidbody;

    protected virtual void Reset() {
        GetComponent<MasterClientOnly>().TargetBehaviours.Add(this);
    }

    protected virtual void Start() {
        CurrentItem = Socket.GetChildren().Select(transform => transform.gameObject).SingleOrDefault();
        if (CurrentItem == null) {
            GenerateItem();
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log(other);
        if (other.gameObject != CurrentItem) {
            return;
        }

        OnItemTaken();
    }

    private void GenerateItem() {
        CurrentItem = PhotonNetwork.InstantiateRoomObject($"Prefabs/{ItemPrefab.name}", Socket.position, Socket.rotation);
        Debug.Assert(CurrentItem.GetComponent<TransformParentSync>());
        CurrentItem.transform.parent = Socket.transform;
        CurrentItemRigidbody.useGravity = false;
        CurrentItemRigidbody.isKinematic = true;
    }

    private void OnItemTaken() {
        CurrentItemRigidbody.useGravity = true;
        CurrentItemRigidbody.isKinematic = false;
        CurrentItem.transform.parent = transform.parent;

        CurrentItem = null;
        GenerateItem();
    }
}
