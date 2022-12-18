using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryArea : MonoBehaviour {

    [SerializeField]
    private HashSet<Ingredient> CollidingIngredients = new HashSet<Ingredient>();

    public UnityEvent<Recipe> NewDish;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (!PhotonNetwork.IsMasterClient) return;

        Ingredient ing = other.gameObject.GetComponentInParent<Ingredient>();
        Debug.Log(other);
        Debug.Log(ing);

        if (ing != null && !CollidingIngredients.Contains(ing)) {
            CollidingIngredients.Add(ing);
            Invoke("ProcessDish", 0.25f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!PhotonNetwork.IsMasterClient) return;

        Ingredient ing = other.gameObject.GetComponentInParent<Ingredient>();
        Debug.Log(other);
        Debug.Log(ing);

        if (ing != null && CollidingIngredients.Contains(ing)) {
            CollidingIngredients.Remove(ing);
        }
    }

    private void ProcessDish() {
        if (!PhotonNetwork.IsMasterClient || CollidingIngredients.Count == 0) return;

        Debug.Log("Processing Dish");
        Recipe r = new Recipe();
        r.RecipeName = "PlayerDish";
        r.Ingredients = new List<Recipe.Ingredient>();

        string s = "PlayerDish:\n";

        foreach (Ingredient i in CollidingIngredients) {
            Recipe.Ingredient ri = new Recipe.Ingredient();

            ri.CookingState = i.CurrentState.CookingState;
            ri.IngredientName = i.IngredientInfo.DisplayName;
            ri.IsChopped = i.CurrentState.IsChopped;

            r.Ingredients.Add(ri);

            s += "\tIngredient: " + ri.IngredientName + "\n\t\tCookingState: " + ri.CookingState + "\n\t\tIsChopped: " + ri.IsChopped + "\n";

            PhotonNetwork.Destroy(i.gameObject);
        }
        CollidingIngredients.Clear();

        Debug.Log(s);

        NewDish.Invoke(r);
    }
}
