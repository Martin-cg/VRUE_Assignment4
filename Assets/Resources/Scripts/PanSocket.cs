using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PanGrip))]
public class PanSocket : XRSocketInteractor {
    private PanGrip Grip;
    private IXRSelectInteractable LastInteractable;
    private Ingredient CurrentIngredient;

    protected new void Awake() {
        base.Awake();

        Grip = GetComponent<PanGrip>();
    }

    public override bool CanSelect(IXRSelectInteractable interactable) {
        return base.CanSelect(interactable)
            && interactable.transform.gameObject.GetComponent<Ingredient>() is Ingredient ingredient
            && ingredient.IngredientInfo.CanBeCooked
            && ingredient.IngredientInfo.NeedsChoppedToCook == ingredient.CurrentState.IsChopped;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        args.interactableObject.transform.gameObject.SetLayerRecursively(Layers.IngredientsInPan);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        args.interactableObject.transform.gameObject.SetLayerRecursively(Layers.Ingredients);
    }

    private void Update() {
        if (!hasSelection) {
            return;
        }

        if (LastInteractable != firstInteractableSelected) {
            LastInteractable = firstInteractableSelected;
            CurrentIngredient = LastInteractable.transform.gameObject.GetComponent<Ingredient>();
        }

        if (!Grip.interactorsSelecting.Any(interactor => interactor.transform.gameObject.GetComponent<StoveSocket>())) {
            return;
        }

        switch (CurrentIngredient.CurrentState.CookingState) {
            case CookingState.Raw:
                if (!CurrentIngredient.IngredientInfo.CanBeCooked) {
                    return;
                }

                CurrentIngredient.CookingProgess += Time.deltaTime / CurrentIngredient.IngredientInfo.TimeToCook;
                if (CurrentIngredient.CookingProgess >= 1) {
                    CurrentIngredient.CookingProgess = 1;
                    CurrentIngredient.CurrentState = CurrentIngredient.CurrentState.GetAsCooked();
                }
                break;
            case CookingState.Cooked:
                if (!CurrentIngredient.IngredientInfo.CanBeBurnt) {
                    return;
                }

                CurrentIngredient.BurningProgess += Time.deltaTime / CurrentIngredient.IngredientInfo.TimeToBurn;
                if (CurrentIngredient.BurningProgess >= 1) {
                    CurrentIngredient.BurningProgess = 1;
                    CurrentIngredient.CurrentState = CurrentIngredient.CurrentState.GetAsBurnt();
                }
                break;
            case CookingState.Burnt:
                // TODO: spawn fire?
                break;
        }
    }

}
