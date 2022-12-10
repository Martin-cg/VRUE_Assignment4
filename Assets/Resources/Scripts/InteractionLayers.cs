using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public static class InteractionLayers {
    public static readonly int Default = InteractionLayerMask.GetMask("Default");
    public static readonly int UI = InteractionLayerMask.GetMask("UI");
    public static readonly int Player = InteractionLayerMask.GetMask("Player");
    public static readonly int Spectator = InteractionLayerMask.GetMask("Spectator");
    public static readonly int Offline = InteractionLayerMask.GetMask("Offline");
}

