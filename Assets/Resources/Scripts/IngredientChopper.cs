using System.Linq;
using UnityEngine;
using System.Collections;

public class IngredientChopper : MonoBehaviour {
    private static int[] IngredientLayers;
    public float CooldownTime = 0.2f;
    private bool IsCooldown = false;

    protected virtual void Awake() {
        if (IngredientLayers == null) {
            IngredientLayers = new int[] { Layers.Ingredients, Layers.IngredientsOnPlate };
        }
    }

    protected virtual void OnTriggerExit(Collider collider) {
        if (IsCooldown) {
            return;
        }

        if (collider.isTrigger || !collider.gameObject.activeInHierarchy) {
            return;
        }

        if (!IngredientLayers.Contains(collider.gameObject.layer)) {
            return;
        }

        var ingredient = collider.GetComponentInParent<Ingredient>();
        if (!ingredient || !ingredient.IngredientInfo || !ingredient.IngredientInfo.CanBeChooped) {
            return;
        }

        ingredient.OnChop();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown() {
        IsCooldown = true;
        yield return new WaitForSeconds(CooldownTime);
        IsCooldown = false;
    }
}
