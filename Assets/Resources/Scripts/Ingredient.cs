using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRGrabInteractable))]
public class Ingredient : MonoBehaviour, IPunObservable {
    public enum IngredientState {
        Initial,
        Chopped,
        Cooked
    }

    public IngredientState _CurrentState = IngredientState.Initial;
    public IngredientState CurrentState {
        get => _CurrentState;
        set {
            if (value != _CurrentState) {
                return;
            }

            _CurrentState = value;
            UpdateModelState();
        }
    }

    public GameObject InitialStateModel;
    public GameObject ChoppedStateModel;
    public GameObject CookedStateModel;

    // Prevent multiple choppings due to multiple collisions.
    // This enforces Collision Enter => Chop => Collision Exit => Collision Enter => Chop => ...
    private bool ChopFlag = false;

    private void UpdateModelState() {
        InitialStateModel.SetActive(CurrentState == IngredientState.Initial);
        ChoppedStateModel.SetActive(CurrentState == IngredientState.Chopped);
        CookedStateModel.SetActive(CurrentState == IngredientState.Cooked);
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
