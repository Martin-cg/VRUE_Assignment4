using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Plate : RigidbodyContainer {
    public List<GameObject> PresetRoots;
    private List<Preset> Presets = new();
    private HashSet<string> CurrentIngredients = new();

    protected override void Awake() {
        base.Awake();

        foreach (var presetRoot in PresetRoots) {
            var preset = new Preset();
            foreach (var ingredientObject in presetRoot.GetChildren()) {
                var ingredient = ingredientObject.GetComponent<IngredientInfo>();
                var requirement = ingredientObject.GetComponent<IngredientPresetRequirement>();
                preset.Root = ingredientObject;
                preset.Ingredients.Add(ingredient.DisplayName, requirement);
            }
            Presets.Add(preset);
        }
    }

    private Ingredient IngredientFromInteractable(IXRInteractable interactable) => interactable.transform.gameObject.GetComponent<Ingredient>();

    public override bool CanHover(IXRHoverInteractable interactable) {
        return base.CanHover(interactable) && (IsHovering(interactable) || GetFirstPossiblePreset(IngredientFromInteractable(interactable)) != null);
    }
    public override bool CanSelect(IXRSelectInteractable interactable) {
        return base.CanSelect(interactable) && (IsSelecting(interactable) || GetFirstPossiblePreset(IngredientFromInteractable(interactable)) != null);
    }

    private Preset GetFirstPossiblePreset(Ingredient newIngredient) {
        if (newIngredient == null) {
            var a = 1;
        }

        foreach (var preset in Presets) {
            if (preset.Ingredients.Count != CurrentIngredients.Count + 1) {
                continue;
            }

            IngredientPresetRequirement requirement;

            foreach (var ingredient in CurrentIngredients) {
                if (!preset.Ingredients.TryGetValue(ingredient, out requirement)) {
                    goto EndOuterLoop;
                }
            }

            if (!preset.Ingredients.TryGetValue(newIngredient.IngredientInfo.DisplayName, out requirement)) {
                continue;
            }

            if (requirement && newIngredient.CurrentState != new IngredientState(requirement.IsChopped, requirement.CookingState)) {
                continue;
            }

            return preset;

            EndOuterLoop:;
        }

        return null;
    }
    private Preset GetCurrentPreset() {
        // TODO: cache
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
        var preset = GetCurrentPreset();

        base.OnStickObject(obj);

        foreach (var (a, b) in preset.Root.GetAllChildren().Zip(ingredient.IngredientInfo.gameObject.GetAllChildren())) {
            Debug.Log(a + " " + b);
            b.transform.SetLocalPose(a.transform.GetLocalPose());
        }
    }

    protected override void OnUnstickObject(ContainedObject obj) {
        base.OnUnstickObject(obj);


    }

    protected override Pose CaptureAttachPose(ContainedObject obj) {
        // return base.CaptureAttachPose(obj);
        var ingredient = IngredientFromInteractable(obj.Interactable);
        var preset = GetCurrentPreset();
        return preset.Ingredients[ingredient.IngredientInfo.DisplayName].transform.GetLocalPose();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        var ingredient = IngredientFromInteractable(args.interactableObject);
        CurrentIngredients.Add(ingredient.IngredientInfo.name);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        var ingredient = IngredientFromInteractable(args.interactableObject);
        CurrentIngredients.Remove(ingredient.IngredientInfo.name);
    }

    private class Preset {
        public GameObject Root;
        public IDictionary<string, IngredientPresetRequirement> Ingredients = new Dictionary<string, IngredientPresetRequirement>();
    }
}
