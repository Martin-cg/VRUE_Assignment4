using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Character))]
public class LocalCharacter : MonoBehaviour {
    public static LocalCharacter Instance { get; private set; }
    public Character Character;
    public XRRig Rig;
    private List<Component> SyncComponents = new();

    protected virtual void Start() {
        if (Instance != null && Instance != this && !Instance.IsDestroyed()) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

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

    protected virtual void OnDestroy() {
        foreach (var component in SyncComponents) {
            Destroy(component);
        }
        SyncComponents.Clear();
    }

    private void SyncTransforms(GameObject source, GameObject target) {
        SyncTransforms(source.transform, target.transform);
    }

    private void SyncTransforms(Transform source, Transform target) {
        var transformSync = source.AddComponent<TransformSync>();
        transformSync.Target = target;
        SyncComponents.Add(transformSync);
    }

    private void SyncAnimations(ActionBasedController controller, GameObject animatedObject) {
        var animationSync = controller.AddComponent<ControllerAnimationSync>();
        animationSync.Controller = controller;
        animationSync.Animator = animatedObject.GetComponentInChildren<Animator>();
        SyncComponents.Add(animationSync);
    }
}
