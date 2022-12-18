using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IngredientBox : MonoBehaviourGameStateCallbacks, IPunObservable {
    public GameObject IngredientPrefab;
    public IngredientInfo IngredientInfoPrefab;
    public Transform Socket;

    private GameObject _CurrentIngredient;
    public GameObject CurrentIngredient {
        get => _CurrentIngredient;
        set {
            _CurrentIngredient = value;
            if (value) {
                CurrentIngredientInteractable = CurrentIngredient.GetComponent<XRBaseInteractable>();
            } else {
                CurrentIngredientInteractable = null;
            }
        }
    }
    private XRBaseInteractable CurrentIngredientInteractable;
    private int CurrentIngredientColliders;
    private ICollection<Collider> BlockingColliders = new HashSet<Collider>();

    protected virtual void Awake() {
        MasterClientOnly.AddTo(this);
    }

    protected virtual void Start() {
        Debug.Assert(IngredientPrefab.GetComponent<XRBaseInteractable>());
        Debug.Assert(IngredientPrefab.GetComponent<Rigidbody>());
        Debug.Assert(IngredientPrefab.GetComponent<TemporaryObject>());

        if (CurrentIngredient) {
            Destroy(CurrentIngredient);
            GenerateIngredient();
        }
    }

    public override void OnGameStarted() {
        base.OnGameStarted();

        GenerateIngredient();
    }

    public override void OnGameStopped() {
        base.OnGameStarted();

        if (CurrentIngredient) {
            PhotonNetwork.Destroy(CurrentIngredient);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (CurrentIngredient && other.attachedRigidbody?.gameObject == CurrentIngredient) {
            CurrentIngredientColliders++;
        } else if (!IsOwnCollider(other) && !other.isTrigger) {
            BlockingColliders.Add(other);
        }
    }
    public void OnTriggerExit(Collider other) {
        if (CurrentIngredient && other.attachedRigidbody?.gameObject == CurrentIngredient) {
            CurrentIngredientColliders--;

            if (CurrentIngredientColliders == 0) {
                // for some reason the item exits once when grabbed and then immediatly reenters for a few frames before exiting again...
                StartCoroutine(ItemMovedAwayReentrancyProtection());
            }
        } else {
            BlockingColliders.Remove(other);
            if (CurrentIngredient == null) {
                GenerateIngredient();
            }
        }
    }

    private bool IsOwnCollider(Collider collider) {
        return collider.gameObject == gameObject || collider.gameObject.IsChildOf(gameObject);
    }

    private IEnumerator ItemMovedAwayReentrancyProtection() {
        yield return new WaitForSeconds(0.5F);

        if (CurrentIngredientColliders == 0) {
            Debug.Log("Debounce Complete. Generating new ingredient.");
            OnItemMovedAway();
        }
    }

    private void GenerateIngredient() {
        if (!PhotonNetwork.InRoom) {
            return;
        }

        // Remove destroyed colliders
        foreach (var collider in BlockingColliders) {
            if (!collider) {
                BlockingColliders.Remove(collider);
            }
        }

        if (BlockingColliders.Count != 0) {
            return;
        }

        // Debug.Log("GenerateItem() " + Socket.transform.position);
        var ingredientInfoPrefabPath = ResourcePathUtils.GetPrefabPath(IngredientInfoPrefab.gameObject);
        CurrentIngredient = PhotonNetwork.InstantiateRoomObject(ResourcePathUtils.GetPrefabPath(IngredientPrefab), Socket.position, Socket.rotation, 0, new object[] { ingredientInfoPrefabPath });

        CurrentIngredientInteractable.selectEntered.AddListener(OnCurrentSelectEntered);
    }

    private void OnCurrentSelectEntered(SelectEnterEventArgs args) => OnCurrentItemGrabbed();

    private void OnCurrentItemGrabbed() {
        // Debug.Log("OnCurrentItemGrabbed()");
        CurrentIngredientInteractable.selectEntered.RemoveListener(OnCurrentSelectEntered);
        CurrentIngredientInteractable.selectExited.AddListener(OnTakenItemReleased);
    }

    private void OnTakenItemReleased(SelectExitEventArgs args) {
        // Debug.Log("OnTakenItemReleased()");
        var interactable = args.interactableObject;
        var obj = interactable.transform.gameObject;

        interactable.selectExited.RemoveListener(OnTakenItemReleased);
    }

    private void OnItemMovedAway() {
        // Debug.Log("OnItemMovedAway()");

        CurrentIngredient = null;
        GenerateIngredient();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            int? viewId = CurrentIngredient ? PhotonView.Get(CurrentIngredient).ViewID : null;
            stream.SendNext(viewId);
        } else {
            int? viewId = stream.ReceiveNext<int?>();
            CurrentIngredient = viewId.HasValue ? PhotonView.Find(viewId.Value).gameObject : null;
        }
    }
}
