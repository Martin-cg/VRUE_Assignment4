using UnityEngine;

public class IngredientInfo : MonoBehaviour {
    public string DisplayName;
    public int NumberOfCuts;
    public GameObject RawModel;
    public GameObject ChoppedModel;
    public Material CookedMaterial;
    public Material BurntMaterial;
    public bool CanBeChooped;
    public bool CanBeCooked;
    public bool CanBeBurnt;
    public float TimeToCook;
    public float TimeToBurn;
    public bool NeedsChoppedToCook;
}
