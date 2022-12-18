using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Resources.Scripts {
    public class CustomXRGrabInteractable : XRGrabInteractable {

        private List<IXRSelectInteractor> BlockedInteractors = new();

        [SerializeField]
        bool WasKinematicValue;

        public override bool IsSelectableBy(IXRSelectInteractor interactor) {
            return base.IsSelectableBy(interactor) && !BlockedInteractors.Contains(interactor);
        }

        public void BlockInteractor(IXRSelectInteractor interactor) => BlockedInteractors.Add(interactor);
        public void UnblockInteractor(IXRSelectInteractor interactor) => BlockedInteractors.Remove(interactor);
        public void BlockInteractorFor(IXRSelectInteractor interactor, float time) {
            IEnumerator DelayedUnblock() {
                yield return new WaitForSeconds(time);
                UnblockInteractor(interactor);
            }

            BlockInteractor(interactor);
            StartCoroutine(DelayedUnblock());
        }

        protected override void SetupRigidbodyGrab(Rigidbody rigidbody) {
            rigidbody.isKinematic = WasKinematicValue;
            base.SetupRigidbodyGrab(rigidbody);
        }

        protected override void SetupRigidbodyDrop(Rigidbody rigidbody) {
            rigidbody.isKinematic = WasKinematicValue;
            base.SetupRigidbodyDrop(rigidbody);
            rigidbody.isKinematic = WasKinematicValue;
            if (forceGravityOnDetach) {
                rigidbody.useGravity = true;
            }
        }

    }
}
