using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HtcViveControllerAnimator : MonoBehaviour {
    public Animator Animator;
    public ActionBasedController Controller;

    private void Start() {
        Controller = Controller != null ? Controller : GetComponentInParent<ActionBasedController>();
        Animator = Animator != null ? Animator : GetComponent<Animator>();
    }

    private void Update() {
        if (Controller && Animator) {
            var triggerValue = Controller.activateActionValue.action.ReadValue<float>();
            Animator.SetFloat("Trigger", triggerValue);

            var gripValue = Controller.selectActionValue.action.ReadValue<float>();
            Animator.SetFloat("Grip", gripValue);
        }
    }
}
