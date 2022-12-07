using UnityEngine;

public static class InteractionLayers {
    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int UI = LayerMask.NameToLayer("UI");
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int Spectator = LayerMask.NameToLayer("Spectator");
}

