using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Character))]
public class LocalCharacter : Singleton<LocalCharacter> {
    public Character Character;
    public XRRig Rig;

    public void Start() {
        Character = GetComponent<Character>();

        foreach (var renderer in Character.Head.GetComponentsInChildren<Renderer>()) {
            renderer.enabled = false;
        }

        SyncTransforms(Rig.transform, Character.Root.transform);
        SyncTransforms(Rig.Head.gameObject, Character.Head);
        SyncTransforms(Rig.LeftHand.gameObject, Character.LeftHand);
        SyncTransforms(Rig.RightHand.gameObject, Character.RightHand);

        SyncAnimations(Rig.LeftHand, Character.LeftHand);
        SyncAnimations(Rig.RightHand, Character.RightHand);
    }

    private void SyncTransforms(GameObject source, GameObject target) {
        SyncTransforms(source.transform, target.transform);
    }

    private void SyncTransforms(Transform source, Transform target) {
        var transformSync = source.AddComponent<TransformSync>();
        transformSync.Target = target;
    }

    private void SyncAnimations(ActionBasedController controller, GameObject animatedObject) {
        var animationSync = controller.AddComponent<ControllerAnimationSync>();
        animationSync.Controller = controller;
        animationSync.Animator = animatedObject.GetComponentInChildren<Animator>();
    }
}
