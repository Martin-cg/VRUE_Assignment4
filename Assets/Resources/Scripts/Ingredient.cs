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

    // Prevent multiple choppings due to multiple collisions.
    // This enforces Collision Enter => Chop => Collision Exit => Collision Enter => Chop => ...
    private bool ChopFlag = false;

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

    private void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.CompareTag(Tags.KnifeBlade)) {
            ChopFlag = false;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.CompareTag(Tags.KnifeBlade)) {
            if (!ChopFlag) {
                Debug.Log("CHOP");

                switch (CurrentState) {
                    case IngredientState.Initial:
                        CurrentState = IngredientState.Chopped;
                        break;
                    case IngredientState.Chopped:
                        CurrentState = IngredientState.Cooked;
                        break;
                    case IngredientState.Cooked:
                        CurrentState = IngredientState.Initial;
                        break;
                    default:
                        CurrentState = IngredientState.Initial;
                        break;
                }

                ChopFlag = true;
            }
        }
    }
}
