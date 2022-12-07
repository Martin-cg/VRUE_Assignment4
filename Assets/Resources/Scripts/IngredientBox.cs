using Photon.Pun;
using System.Collections;
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
        Socket.selectEntered.AddListener(OnSelectEnter);
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
    private void OnSelectEnter(SelectEnterEventArgs args) {
        OnItemEntered();
    }

    private void OnItemTaken() {
        Debug.Log("Item Taken");
        //CurrentItem.GetComponent<Collider>().enabled = true;
        CurrentItem = null;
        StartCoroutine(GenerateItem());
    }

    private void OnItemEntered() {
        Debug.Log("Item Entered");
        CurrentItem = Socket.interactablesSelected.Select(interactable => interactable.transform.gameObject).FirstOrDefault();
    }

    private IEnumerator GenerateItem() {
        yield return new WaitForSeconds(1);

        if (CurrentItem != null) {
            yield break;
        }

        Debug.Log("NEW ITEM");
        CurrentItem = PhotonNetwork.InstantiateRoomObject($"Prefabs/{ItemPrefab.name}", Socket.attachTransform.position, Socket.attachTransform.rotation);
        //CurrentItem.GetComponent<Collider>().enabled = false;
        Socket.interactionManager.SelectEnter(Socket, (IXRSelectInteractable) CurrentItem.GetComponent<XRGrabInteractable>());
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("ENTER", other);
    }

    private void OnTriggerStay(Collider other) {
        //Debug.Log("STAY", other);
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("EXIT", other);
    }
}
