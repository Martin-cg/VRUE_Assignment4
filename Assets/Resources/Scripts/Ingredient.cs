using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Ingredient : MonoBehaviour, IPunObservable {

    public enum IngredientState {
        Initial,
        Chopped,
        Cooked
    }

    public IngredientState CurrentState = IngredientState.Initial;

    public GameObject InitialStateModel;
    public GameObject ChoppedStateModel;
    public GameObject CookedStateModel;

    private void Update() {
        HideAllStates();
        ShowActiveState();
    }

    private void HideAllStates() {
        InitialStateModel.SetActive(false);
        ChoppedStateModel.SetActive(false);
        CookedStateModel.SetActive(false);
    }

    private void ShowActiveState() {
        switch (CurrentState) {
            case IngredientState.Initial:
                InitialStateModel.SetActive(true);
                break;
            case IngredientState.Chopped:
                ChoppedStateModel.SetActive(true);
                break;
            case IngredientState.Cooked:
                CookedStateModel.SetActive(true);
                break;
            default:
                InitialStateModel.SetActive(true);
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting)
            stream.SendNext(CurrentState);
        else
            CurrentState = (IngredientState)stream.ReceiveNext();
    }
}
