using ExitGames.Client.Photon;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public readonly struct IngredientState : IEquatable<IngredientState> {
    public static readonly IngredientState RawUnchopped = new IngredientState(false, CookingState.Raw);

    public readonly bool IsChopped;
    public readonly CookingState CookingState;

    public IngredientState(bool isChopped, CookingState cookingState) {
        IsChopped = isChopped;
        CookingState = cookingState;
    }

    public override bool Equals(object obj) => obj is IngredientState state && Equals(state);
    public bool Equals(IngredientState other) => IsChopped == other.IsChopped && CookingState == other.CookingState;
    public override int GetHashCode() => HashCode.Combine(IsChopped, CookingState);
    public static bool operator ==(IngredientState left, IngredientState right) => left.Equals(right);
    public static bool operator !=(IngredientState left, IngredientState right) => !(left == right);

    public IngredientState GetAsChopped() => new IngredientState(true, CookingState);
    public IngredientState GetAsCooked() => new IngredientState(IsChopped, CookingState.Cooked);
    public IngredientState GetAsBurnt() => new IngredientState(IsChopped, CookingState.Burnt);

    public static short Serialize(StreamBuffer outStream, object customObject) {
        return (short)Serde.Serialize(outStream, (IngredientState)customObject);
    }

    public static object Deserialize(StreamBuffer inStream, short length) {
        Debug.Assert(length == Marshal.SizeOf(typeof(IngredientState)));
        return Serde.Deserialize<IngredientState>(inStream);
    }
}
