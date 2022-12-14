using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerAnimationSync : MonoBehaviour {
    public Animator Animator;
    public ActionBasedController Controller;

    private void Start() {
        if (Animator && Controller) {
            var action = Controller.selectAction.reference.action;
            action.Enable();
            action.performed += e => OnStartGrip();
            action.canceled += e => OnEndGrip();
        }
    }

    private void OnStartGrip() {
        if (Animator) {
            Animator.SetInteger("AnimationState", 1);
        }
    }

    private void OnEndGrip() {
        if (Animator) {
            Animator.SetInteger("AnimationState", 0);
        }
    }
} 
