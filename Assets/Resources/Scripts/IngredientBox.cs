using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IngredientBox : MonoBehaviourPun {
    public GameObject ItemPrefab;
    public GameObject CurrentItem;
    public XRSocketInteractor Socket;

    protected virtual void Reset() {
        FindSocket();

        GetComponent<MasterClientOnly>().TargetBehaviours.Add(this);
    }

    protected virtual void Start() {
        FindSocket();

        // Do we filter the items here?
        CurrentItem = Socket.interactablesSelected.Select(interactable => interactable.transform.gameObject).FirstOrDefault();
        if (CurrentItem == null) {
            GenerateItem();
        }

        Socket.selectExited.AddListener(OnSelectExit);
    }

    protected virtual void OnDestroy() {
        Socket.selectExited.RemoveListener(OnSelectExit);
    }

    private void FindSocket() {
        Socket = Socket == null ? GetComponent<XRSocketInteractor>() : Socket;
        Socket = Socket == null ? GetComponentInChildren<XRSocketInteractor>() : Socket;
    }

    private void OnSelectExit(SelectExitEventArgs args) {
        OnItemTaken();
    }

    private void OnItemTaken() {
        CurrentItem = null;
        GenerateItem();
    }

    private void GenerateItem() {
        CurrentItem = PhotonNetwork.InstantiateRoomObject($"Prefabs/{ItemPrefab.name}", Socket.attachTransform.position, Socket.attachTransform.rotation);
        Socket.interactionManager.SelectEnter(Socket, (IXRSelectInteractable) CurrentItem.GetComponent<XRGrabInteractable>());
    }
}
