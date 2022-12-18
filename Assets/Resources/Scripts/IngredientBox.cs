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
        }
    }
    private List<Collider> BlockingColliders = new();

    protected virtual void Awake() {
        MasterClientOnly.AddTo(this);
    }

    protected virtual void Start() {
        Debug.Assert(IngredientPrefab.GetComponent<XRBaseInteractable>());
        Debug.Assert(IngredientPrefab.GetComponent<Rigidbody>());
        Debug.Assert(IngredientPrefab.GetComponent<TemporaryObject>());
    }

    public override void OnGameStarted() {
        base.OnGameStarted();

        if (PhotonNetwork.IsMasterClient) {
            GenerateIngredient();
        }
    }

    public override void OnGameStopped() {
        base.OnGameStarted();

        if (PhotonNetwork.IsMasterClient) {
            if (CurrentIngredient) {
                PhotonNetwork.Destroy(CurrentIngredient);
            }
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (!IsOwnCollider(other) && !other.isTrigger) {
            BlockingColliders.Add(other);
        }
    }
    public void OnTriggerExit(Collider other) {
        BlockingColliders.Remove(other);
        BlockingColliders.RemoveAll(collider => !collider || !collider.enabled || !collider.gameObject.activeInHierarchy);

        if (BlockingColliders.Count == 0) {
            StartCoroutine(GenerateIngredientIfNoReentrancy());
        }
    }

    private bool IsOwnCollider(Collider collider) {
        return collider.gameObject == gameObject || collider.gameObject.IsChildOf(gameObject);
    }

    private IEnumerator GenerateIngredientIfNoReentrancy() {
        yield return new WaitForSeconds(0.5F);

        if (BlockingColliders.Count == 0) {
            // Debug.Log("Debounce Complete. Generating new ingredient.");
            GenerateIngredient();
        }
    }

    private void GenerateIngredient() {
        if (!PhotonNetwork.InRoom || BlockingColliders.Count != 0) {
            return;
        }

        // Debug.Log("GenerateItem() " + Socket.transform.position);
        var ingredientInfoPrefabPath = ResourcePathUtils.GetPrefabPath(IngredientInfoPrefab.gameObject);
        CurrentIngredient = PhotonNetwork.InstantiateRoomObject(ResourcePathUtils.GetPrefabPath(IngredientPrefab), Socket.position, Socket.rotation, 0, new object[] { ingredientInfoPrefabPath });
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
