using System.Collections.Generic;
using UnityEngine;

public class IngredientChopper : MonoBehaviour {
    private readonly Dictionary<Ingredient, int> Chopping = new();

    protected virtual void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer != Layers.Ingredients) {
            return;
        }

        var ingredient = collider.GetComponentInParent<Ingredient>();
        if (!ingredient || !ingredient.IngredientInfo || !ingredient.IngredientInfo.CanBeChooped) {
            return;
        }

        var added = Chopping.AddOrUpdate(ingredient, 1, count => count+1);
        if (added) {
            ingredient.OnChopBegin();
        }
    }

    protected virtual void OnTriggerExit(Collider collider) {
        if (collider.gameObject.layer != Layers.Ingredients) {
            return;
        }

        var ingredient = collider.GetComponentInParent<Ingredient>();
        if (!ingredient || !ingredient.IngredientInfo || !ingredient.IngredientInfo.CanBeChooped) {
            return;
        }

        var count = Chopping[ingredient];
        if (count <= 1) {
            Chopping.Remove(ingredient);
            ingredient.OnChopEnd();
        } else {
            Chopping[ingredient] = count - 1;
        }
    }
}
