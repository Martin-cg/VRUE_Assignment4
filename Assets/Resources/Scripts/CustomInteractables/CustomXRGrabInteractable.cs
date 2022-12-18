using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
