using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OrderManager : MonoBehaviourGameStateCallbacks, IPunObservable {

    [SerializeField]
    private GameObject PlatePrefab;
    private Plate PlateInstance;
    private bool Initialized = false;

    private List<Recipe> Recipes;

    private long LastOrderGenerationTimestamp = 0;

    private bool IsGeneratingOrders = false;

    public List<Order> Orders = new List<Order>();

    public List<RecipeUIController> RecipeUISlots;

    public long OrderAddInterval = 15000;
    public int MaxConcurrentOrders = 5;

    public long OrderExpiryBase = 10000;
    public long OrderExpiryIngredientScale = 20000;

    public float PointsMultiplier = 10.0f;

    public UnityEvent<float> PointsAwarded;

    public override void OnGameStarted() {
        base.OnGameStarted();
        IsGeneratingOrders = true;
    }

    public override void OnGameStopped() {
        base.OnGameStopped();
        IsGeneratingOrders = false;
        Orders.Clear();
        LastOrderGenerationTimestamp = 0;
    }

    private void Awake() {
        
    }

    private void Start() {
        // InvokeRepeating("PrintDebugOutput", 1f, 1f);
    }

    private void Update() {
        if (!Initialized) {
            Initialize();
            return;
        }

        if (IsGeneratingOrders)
            TryAddNewOrder();

        if (Orders.Count > 0) {

            if (PhotonNetwork.IsMasterClient) {
                Orders.RemoveAll(o => o.HasExpired);

                Orders.Sort((a, b) => b.ExpirationProgress.CompareTo(a.ExpirationProgress));
            }

            for (int i = 0; i < RecipeUISlots.Count; i++) {
                RecipeUIController r = RecipeUISlots[i];
                Order o = null;

                if (i < Orders.Count) {
                    o = Orders[i];
                }

                if (o != null) {
                    r.RecipeName = o.Recipe.RecipeName;
                    r.Progress = 1.0f - o.ExpirationProgress;
                } else {
                    r.Progress = 1.0f;
                }
            }

        }
    }

    public void HandleNewDish(Recipe NewDish) {
        Debug.Log("NEW DISH!!!");

        bool matched = false;
        float points = 0.0F;
        for (int i = 0; i < Orders.Count; i++) {
            Recipe OrderRecipe = Orders[i].Recipe;
            points = (1.0F - Orders[i].ExpirationProgress) * PointsMultiplier;

            if (CheckDishEquality(OrderRecipe, NewDish)) {
                matched = true;
                Orders.Remove(Orders[i]);
                break;
            }
        }

        if (matched) {
            Debug.Log("AWARDING POINTS");
            PointsAwarded.Invoke(points);

        } else {
            Debug.Log("DISH NOT FOUND");
        }
    }

    private bool CheckDishEquality(Recipe r1, Recipe r2) {
        r1.Ingredients.Sort((a, b) => {
            return a.IngredientName.CompareTo(b.IngredientName);
        });

        r2.Ingredients.Sort((a, b) => {
            return a.IngredientName.CompareTo(b.IngredientName);
        });

        Debug.Log("CHECKING DISH 1: " + r1.ToString());
        Debug.Log("AGAINST");
        Debug.Log("CHECKING DISH 2: " + r2.ToString());

        bool matches = true;

        if (r1.Ingredients.Count == r2.Ingredients.Count) {
            for (int i = 0; i < r1.Ingredients.Count; i++) {
                Recipe.Ingredient r1Ingredient = r1.Ingredients[i];
                Recipe.Ingredient r2Ingredient = r2.Ingredients[i];

                if (!r1Ingredient.IngredientName.Equals(r2Ingredient.IngredientName)) {
                    Debug.Log("DISHCHECKER SAYS NO! MISMATCHING INGREDIENT NAME: " + r1Ingredient.IngredientName + " != " + r2Ingredient.IngredientName);
                    matches = false;
                } else if (r1Ingredient.IsChopped != r2Ingredient.IsChopped) {
                    Debug.Log("DISHCHECKER SAYS NO! MISMATCHING INGREDIENT CHOP: " + r1Ingredient.IsChopped + " != " + r2Ingredient.IsChopped);
                    matches = false;
                } else if (r1Ingredient.CookingState != r2Ingredient.CookingState) {
                    Debug.Log("DISHCHECKER SAYS NO! MISMATCHING INGREDIENT COOKINGSTATE: " + r1Ingredient.CookingState + " != " + r2Ingredient.CookingState);
                    matches = false;
                }
            }

        } else {
            Debug.Log("DISHCHECKER SAYS NO! MISMATCHING INGREDIENT COUNT: " + r1.Ingredients.Count + " != " + r2.Ingredients.Count);
            matches = false;
        }

        return matches;
    }

    private void PrintDebugOutput() {
        if (Orders.Count > 0) {
            Debug.Log("IsMaster? " + PhotonNetwork.IsMasterClient);
            Orders.ForEach(o => {
                Debug.Log(string.Format("{0}: Expired? {1} ExpirationProgress: {2}", o.Recipe.RecipeName, o.HasExpired, o.ExpirationProgress));
            });
            Debug.Log("\n");
        }
    }

    private void TryAddNewOrder() {
        if (!PhotonNetwork.IsMasterClient) return;

        long CurrentMS = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (Orders.Count < MaxConcurrentOrders && CurrentMS - OrderAddInterval >= LastOrderGenerationTimestamp) {
            LastOrderGenerationTimestamp = CurrentMS;

            System.Random rnd = new System.Random();
            Recipe r = Recipes[rnd.NextInt(0, Recipes.Count - 1)];

            Orders.Add(new Order(r, r.Ingredients.Count * 20000 + 10000));
        }
    }

    private void Initialize() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        PlateInstance = Instantiate(PlatePrefab).GetComponent<Plate>();
        PlateInstance.name = "OrderManagerPlateInstance";
        PlateInstance.gameObject.SetActive(false);
        PlateInstance.gameObject.transform.SetParent(gameObject.transform);

        Recipes = PlateInstance.Presets.ConvertAll(p => {
            Recipe r = new Recipe();
            r.RecipeName = p.Root.name;

            r.Ingredients = p.Ingredients.ToList().ConvertAll(entry => {
                Recipe.Ingredient i = new Recipe.Ingredient();
                i.IngredientName = entry.Key;
                i.IsChopped = entry.Value.Requirement.IsChopped;
                i.CookingState = entry.Value.Requirement.CookingState;

                return i;
            });

            return r;
        });

        Initialized = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            Orders.Sort((a, b) => b.ExpirationProgress.CompareTo(a.ExpirationProgress));

            stream.SendNext(Initialized);
            stream.SendNext(LastOrderGenerationTimestamp);
            stream.SendNext(IsGeneratingOrders);
            
            int RecipeCount = Recipes.Count;
            stream.SendNext(RecipeCount);
            for (int i = 0; i < RecipeCount; i++) {
                Recipes[i].Serialize(stream);
            }

            int OrderCount = Orders.Count;
            stream.SendNext(OrderCount);
            for (int i = 0; i < OrderCount; i++) {
                Orders[i].Serialize(stream);
            }
        } else {
            Initialized = stream.ReceiveNext<bool>();
            LastOrderGenerationTimestamp = stream.ReceiveNext<long>();
            IsGeneratingOrders = stream.ReceiveNext<bool>();

            int RecipeCount = stream.ReceiveNext<int>();
            Recipes = new List<Recipe>();
            for (int i = 0; i < RecipeCount; i++) {
                Recipe r = Recipe.Deserialize(stream);
                Recipes.Add(r);
            }

            int OrderCount = stream.ReceiveNext<int>();
            Orders = new List<Order>();
            for (int i = 0; i < OrderCount; i++) {
                Order o = Order.Deserialize(stream);
                Orders.Add(o);
            }
        }
    }
}
