using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Plate : RigidbodyContainer, IPunObservable {
    public GameObject PresetRoot;
    private List<Preset> Presets = new();
    private HashSet<string> CurrentIngredients = new();
    private Dictionary<GameObject, Pose> AttachPose = new();

    protected override void Awake() {
        base.Awake();

        foreach (var presetRoot in PresetRoot.GetChildren()) {
            var preset = new Preset {
                Root = presetRoot
            };
            foreach (var ingredientObject in presetRoot.GetChildren()) {
                var ingredient = ingredientObject.GetComponent<IngredientInfo>();
                var requirement = ingredientObject.GetComponent<IngredientPresetRequirement>();
                preset.Ingredients.Add(ingredient.DisplayName, new PresetIngredientiInfo() {
                    Object = ingredientObject,
                    Requirement = requirement
                });
            }
            Presets.Add(preset);
        }
    }

    private Ingredient IngredientFromInteractable(IXRInteractable interactable) => interactable.transform.gameObject.GetComponent<Ingredient>();
    private IngredientInfo IngredientInfoFromInteractable(IXRInteractable interactable) => IngredientFromInteractable(interactable).IngredientInfo;

    public override bool CanHover(IXRHoverInteractable interactable) {
        return base.CanHover(interactable) && (IsHovering(interactable) || GetFirstPossiblePreset(IngredientFromInteractable(interactable)) != null);
    }
    public override bool CanSelect(IXRSelectInteractable interactable) {
        return base.CanSelect(interactable) && (IsSelecting(interactable) || (!interactable.isSelected && GetFirstPossiblePreset(IngredientFromInteractable(interactable)) != null));
    }

    private Preset GetFirstPossiblePreset(Ingredient newIngredient) {
        if (CurrentIngredients.Contains(newIngredient.IngredientInfo.DisplayName)) {
            return null;
        }

        foreach (var preset in Presets) {
            if (preset.Ingredients.Count != CurrentIngredients.Count + 1) {
                continue;
            }

            PresetIngredientiInfo presetIngredientInfo;

            foreach (var ingredient in CurrentIngredients) {
                if (!preset.Ingredients.TryGetValue(ingredient, out presetIngredientInfo)) {
                    goto EndOuterLoop;
                }
            }

            if (!preset.Ingredients.TryGetValue(newIngredient.IngredientInfo.DisplayName, out presetIngredientInfo)) {
                continue;
            }

            var requirement = presetIngredientInfo.Requirement;
            if (requirement && newIngredient.CurrentState != new IngredientState(requirement.IsChopped, requirement.CookingState)) {
                continue;
            }

            return preset;

            EndOuterLoop:;
        }

        return null;
    }
    private Preset GetCurrentPreset() {
        foreach (var preset in Presets) {
            if (preset.Ingredients.Count != CurrentIngredients.Count) {
                continue;
            }

            foreach (var ingredient in CurrentIngredients) {
                if (!preset.Ingredients.ContainsKey(ingredient)) {
                    goto EndOuterLoop;
                }
            }

            return preset;

            EndOuterLoop:;
        }

        return null;
    }

    protected override void OnStickObject(ContainedObject obj) {
        var ingredient = IngredientFromInteractable(obj.Interactable);
        CurrentIngredients.Add(ingredient.IngredientInfo.DisplayName);

        ApplyCurrentPreset();
        base.OnStickObject(obj);
    }

    private void ApplyCurrentPreset() {
        var preset = GetCurrentPreset();

        foreach (var containedObject in Contents.Values) {
            var ingredient = IngredientInfoFromInteractable(containedObject.Interactable);
            var presetRoot = preset.Ingredients[ingredient.DisplayName].Object;

            foreach (var (presetObject, targetObject) in presetRoot.GetAllChildren().Zip(ingredient.gameObject.GetAllChildren())) {
                Debug.Assert(presetObject.name == targetObject.name);
                AttachPose.TryAdd(targetObject, targetObject.transform.GetLocalPose());
                targetObject.transform.SetLocalPose(presetObject.transform.GetLocalPose());
            }
        }
    }

    protected override void OnUnstickObject(ContainedObject obj) {
        base.OnUnstickObject(obj);

        var ingredient = IngredientFromInteractable(obj.Interactable);

        // revert preset pose for current preset
        var ingredientModelRoot = ingredient.IngredientInfo.gameObject;
        foreach (var targetObject in ingredientModelRoot.GetAllChildren()) {
            var removed = AttachPose.Remove(targetObject, out var attachPose);
            Debug.Assert(removed);
            targetObject.transform.SetLocalPose(attachPose);
        }

        // remove ingredient and apply possible preset
        CurrentIngredients.Remove(ingredient.IngredientInfo.DisplayName);

        ApplyCurrentPreset();
    }

    protected override Pose CaptureAttachPose(ContainedObject obj) {
        // return base.CaptureAttachPose(obj);
        var ingredient = IngredientFromInteractable(obj.Interactable);
        var preset = GetCurrentPreset();
        return preset.Ingredients[ingredient.IngredientInfo.DisplayName].Object.transform.GetLocalPose();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        base.OnPhotonSerializeView(stream, info);

        if (stream.IsWriting) {
            stream.SendNext(CurrentIngredients.Count);
            foreach (var name in CurrentIngredients) {
                stream.SendNext(name);
            }

            stream.SendNext(AttachPose.Count);
            foreach (var (obj, pose) in AttachPose) {
                var (path, view) = PhotonSerdeUtils.GetPhotonViewRelativeScenePath(obj);
                stream.SendNext(view.sceneViewId);
                stream.SendNext(path);
                stream.SendNext(pose);
            }
        } else {
            CurrentIngredients.Clear();
            var count = stream.ReceiveNext<int>();
            for (var i = 0; i < count; i++) {
                 CurrentIngredients.Add(stream.ReceiveNext<string>());
            }

            AttachPose.Clear();
            count = stream.ReceiveNext<int>();
            for (var i = 0; i < count; i++) {
                var sceneViewId = stream.ReceiveNext<int>();
                var path = stream.ReceiveNext<string[]>();
                var pose = stream.ReceiveNext<Pose>();
                var target = PhotonSerdeUtils.ResolvePhotonViewRelativeScenePath(sceneViewId, path);
                AttachPose.Add(target, pose);
            }
        }
    }

    private class Preset {
        public GameObject Root;
        public IDictionary<string, PresetIngredientiInfo> Ingredients = new Dictionary<string, PresetIngredientiInfo>();
    }
    private class PresetIngredientiInfo {
        public GameObject Object;
        public IngredientPresetRequirement Requirement;
    }
}
