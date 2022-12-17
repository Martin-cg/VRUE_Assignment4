using UnityEngine;

public static class Layers {
    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int AlwaysInFront = LayerMask.NameToLayer("AlwaysInFront");
    public static readonly int Ingredients = LayerMask.NameToLayer("Ingredients");
    public static readonly int KnifeBlade = LayerMask.NameToLayer("KnifeBlade");
    public static readonly int IngredientsOnPlate = LayerMask.NameToLayer("IngredientsOnPlate");
}

