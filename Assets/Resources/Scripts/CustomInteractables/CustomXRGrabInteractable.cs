using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Resources.Scripts {
    public class CustomXRGrabInteractable : XRGrabInteractable {

        [SerializeField]
        bool WasKinematicValue;

        protected override void SetupRigidbodyGrab(Rigidbody rigidbody) {
            rigidbody.isKinematic = WasKinematicValue;
            base.SetupRigidbodyGrab(rigidbody);
        }

        protected override void SetupRigidbodyDrop(Rigidbody rigidbody) {
            base.SetupRigidbodyDrop(rigidbody);
            rigidbody.isKinematic = WasKinematicValue;
            if (forceGravityOnDetach) {
                rigidbody.useGravity = true;
            }
        }

    }
}
