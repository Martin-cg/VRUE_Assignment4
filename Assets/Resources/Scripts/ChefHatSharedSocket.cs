using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

[DisallowMultipleComponent]
public class ChefHatSharedSocket : XRSocketInteractor {
    public override void GetValidTargets(List<IXRInteractable> targets) {
        base.GetValidTargets(targets);

        for (int i=targets.Count-1; i>=0; i--) {
            var interactable = targets[i];
            if (!interactable.transform.HasComponent<ChefHat>()) {
                targets.RemoveAt(i);
            }
        }
    }
}
