﻿using Photon.Pun;
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

    public Color ChopProgressColor;
    public Color CookProgressColor;
    public Color BurnProgressColor;

    public Vector3 ProgressCapsuleOffset;

    public GameObject ProgressCapsulePrefab;
    private GameObject ProgressCapsule;
    private ProgressCapsuleManager ProgressCapsuleManager;
    
    [SerializeField]
    private int RemainingChops;
    public float CookingProgess = 0;
    public float BurningProgess = 0;

    protected virtual void Awake() {
        Interactable.enabled = false;
    }

    protected virtual void Start() {
        if (IngredientInfo != null && !IngredientInfo.transform.IsChildOf(transform)) {
            Initialize(IngredientInfo.gameObject);
        }
        ProgressCapsule = Instantiate(ProgressCapsulePrefab);
        ProgressCapsule.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        ProgressCapsuleManager = ProgressCapsule.GetComponent<ProgressCapsuleManager>();
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

    protected virtual void Update() {
        ProgressCapsule.transform.position = transform.position + ProgressCapsuleOffset;

        float newProgress = 0.0F;
        Color newColor = ChopProgressColor;

        if (IngredientInfo.NumberOfCuts > 0) {
            newProgress = 1.0f - (RemainingChops / (float)IngredientInfo.NumberOfCuts);
            newColor = ChopProgressColor;
        }
        if (RemainingChops == 0 && CookingProgess > 0) {
            newProgress = CookingProgess;
            newColor = CookProgressColor;
        }
        if (CookingProgess == 1 && BurningProgess > 0) {
            newProgress = BurningProgess;
            newColor = BurnProgressColor;
        }

        ProgressCapsuleManager.Progress = newProgress;
        ProgressCapsuleManager.ProgressColor = newColor;
    }

    private void UpdateModelState() {
        var activeModel = CurrentState.IsChopped ? IngredientInfo.ChoppedModel : IngredientInfo.RawModel;
        var unativeModel = activeModel == IngredientInfo.ChoppedModel ? IngredientInfo.RawModel : IngredientInfo.ChoppedModel;

        if (unativeModel) {
            unativeModel.SetActive(false);
        }
        if (activeModel) {
            activeModel.SetActive(true);

            Interactable.colliders.Clear();
            Interactable.colliders.AddRange(activeModel.GetComponentsInChildren<Collider>());
            var interactorsSelecting = Interactable.interactorsSelecting;
            var interactionManager = Interactable.interactionManager;
            interactionManager.UnregisterInteractable(Interactable as IXRSelectInteractable);
            interactionManager.RegisterInteractable(Interactable as IXRSelectInteractable);
            foreach (var interactor in interactorsSelecting) {
                interactionManager.SelectEnter(interactor, Interactable);
            }

            var renderers = activeModel.GetComponentsInChildren<Renderer>();
            switch (CurrentState.CookingState) {
                case CookingState.Raw:
                    // raw material is default and state cannot change back
                    break;
                case CookingState.Cooked:
                    if (CurrentState.IsChopped && IngredientInfo.ChoppedCookedMaterial) {
                        foreach (var renderer in renderers) {
                            renderer.material = IngredientInfo.ChoppedCookedMaterial;
                        }
                    } else if (IngredientInfo.CookedMaterial) {
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
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(CurrentState);
            stream.SendNext(RemainingChops);
            stream.SendNext(CookingProgess);
        } else {
            CurrentState = stream.ReceiveNext<IngredientState>();
            RemainingChops = stream.ReceiveNext<int>();
            CookingProgess = stream.ReceiveNext<float>();
        }
    }

    public void OnChop() {
        if (IngredientInfo.CanBeChooped) {
            photonView.RequestOwnership();
            RemainingChops = Math.Max(0, RemainingChops - 1);
            if (RemainingChops == 0) {
                CurrentState = CurrentState.GetAsChopped();
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        var ingredientPath = info.photonView.InstantiationData[0] as string;
        var prefab = Resources.Load<GameObject>(ingredientPath);
        Initialize(prefab);
    }

    private void Initialize(GameObject prefab) {
        IngredientInfo = Instantiate(prefab, transform).GetComponent<IngredientInfo>();
        RemainingChops = IngredientInfo.NumberOfCuts;
        Interactable.colliders.AddRange(IngredientInfo.GetComponentsInChildren<Collider>().Where(collider => !collider.isTrigger));
        Interactable.enabled = true;
        UpdateModelState();
    }
}
