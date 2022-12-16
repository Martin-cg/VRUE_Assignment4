using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(TemporaryObject))]
public class Ingredient : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback {
    public IngredientInfo IngredientInfo;
    public XRBaseInteractable Interactable;

    private int RemainingChops;

    protected virtual void Awake() {
        Interactable.enabled = false;
    }

    public IngredientState _CurrentState = IngredientState.RawUnchopped;
    public IngredientState CurrentState {
        get => _CurrentState;
        set {
            if (value == _CurrentState) {
                return;
            }

            _CurrentState = value;
            UpdateModelState();
        }
    }

    // Prevent multiple choppings due to multiple collisions.
    // This enforces Collision Enter => Chop => Collision Exit => Collision Enter => Chop => ...
    private bool ChopFlag = false;

    private void UpdateModelState() {
        var activeModel = CurrentState.IsChopped ? IngredientInfo.ChoppedModel : IngredientInfo.RawModel;
        var unativeModel = activeModel == IngredientInfo.ChoppedModel ? IngredientInfo.RawModel : IngredientInfo.ChoppedModel;

        unativeModel.SetActive(false);
        activeModel.SetActive(true);

        var renderers = activeModel.GetComponentsInChildren<Renderer>();
        switch (CurrentState.CookingState) {
            case CookingState.Raw:
                // raw material is default and state cannot change back
                break;
            case CookingState.Cooked:
                if (IngredientInfo.CookedMaterial) {
                    foreach (var renderer in renderers) {
                        renderer.material = IngredientInfo.CookedMaterial;
                    }
                }
                break;
            case CookingState.Burnt:
                if (IngredientInfo.BurntMaterial) {
                    foreach (var renderer in renderers) {
                        renderer.material = IngredientInfo.BurntMaterial;
                    }
                }
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(CurrentState);
            stream.SendNext(RemainingChops);
        } else {
            CurrentState = stream.ReceiveNext<IngredientState>();
            RemainingChops = stream.ReceiveNext<int>();
        }
    }

    public void OnChopBegin() {
        ChopFlag = false;
    }

    public void OnChopEnd() {
        if (!ChopFlag) {
            Debug.Log("CHOP");
            photonView.RequestOwnership();
            RemainingChops = Math.Max(0, RemainingChops - 1);
            if (RemainingChops == 0) {
                CurrentState = CurrentState.GetAsChopped();
            }
            ChopFlag = true;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        var ingredientPath = info.photonView.InstantiationData[0] as string;
        var prefab = Resources.Load<GameObject>(ingredientPath);
        IngredientInfo = Instantiate(prefab, transform).GetComponent<IngredientInfo>();
        RemainingChops = IngredientInfo.NumberOfCuts;
        Interactable.colliders.AddRange(IngredientInfo.GetComponentsInChildren<Collider>().Where(collider => !collider.isTrigger));
        Interactable.enabled = true;
        UpdateModelState();
    }
}
