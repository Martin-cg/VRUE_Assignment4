using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyContainer : XRSocketInteractor, IPunObservable {
    protected readonly IDictionary<GameObject, ContainedObject> Contents = new Dictionary<GameObject, ContainedObject>();

    protected override void Awake() {
        base.Awake();

        if (GetComponent<XRBaseInteractable>()) {
            gameObject.GetOrAddComponent<SynchronizedMoveableSocketInteractor>();
        }
    }

    public override Transform GetAttachTransform(IXRInteractable interactable) {
        return Contents.TryGetValue(interactable.transform.gameObject, out var obj) ? obj.AttachTransform : base.GetAttachTransform(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable) {
        // TODO: maybe limit to interactables that have been recently touched.
        return (!interactable.isSelected || (IsSelecting(interactable) && interactable.interactorsSelecting.Count == 1)) && interactable.transform.gameObject.HasComponent<Ingredient>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        StickObject(args.interactableObject.transform.gameObject);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        UnstickObject(args.interactableObject.transform.gameObject);
    }

    private void OnJointBreak(float breakForce) {
        var expectedJoints = Contents.Values.ToDictionary(e => e.Joint);

        foreach (var joint in GetComponents<FixedJoint>()) {
            if (expectedJoints.TryGetValue(joint, out var obj)) {
                interactionManager.SelectExit(this, obj.Interactable as IXRSelectInteractable);
                UnstickObject(obj);
            } else {
                Destroy(joint);
            }
        }
    }

    public void StickObject(GameObject gameObject) {
        StickObject(gameObject, out var _);
    }
    protected void StickObject(GameObject gameObject, out ContainedObject containedObject) {
        var obj = new ContainedObject(gameObject) {
            Rigidbody = gameObject.GetComponent<Rigidbody>(),
            PhotonView = gameObject.GetComponent<PhotonView>(), // We use this istead of PhotonView.Get(gameObject) because we want to assert that the view is present in the object itself.
            Interactable = gameObject.GetComponent<XRGrabInteractable>(),
            AttachTransform = new GameObject($"[Attach] {gameObject.name}").transform,
            Joint = this.gameObject.AddComponent<FixedJoint>()
        };
        obj.AttachTransform.parent = transform;

        obj.Joint.enableCollision = false;
        obj.Joint.autoConfigureConnectedAnchor = true;
        obj.Joint.connectedBody = obj.Rigidbody;
        obj.Joint.breakForce = 10;

        Debug.Assert(obj.Rigidbody != null, $"No {obj.Rigidbody.GetType().Name} on contained object", gameObject);
        Debug.Assert(obj.PhotonView != null, $"No {obj.PhotonView.GetType().Name} on contained object", gameObject);
        Debug.Assert(obj.Interactable != null, $"No {obj.Interactable.GetType().Name} on contained object", gameObject);

        var added = Contents.TryAdd(gameObject, obj);
        if (!added) {
            Debug.LogWarning("Tried to add object to container while already contained.", gameObject);
            containedObject = null;
            return;
        }

        OnStickObject(obj);
        containedObject = obj;
    }

    protected virtual void OnStickObject(ContainedObject obj) {
        obj.PhotonView.RequestOwnership();

        obj.Rigidbody.isKinematic = false;
        obj.Rigidbody.useGravity = true;

        // TODO resolve existing collision

        var attachPose = CaptureAttachPose(obj);
        obj.AttachTransform.localPosition = attachPose.position;
        obj.AttachTransform.localRotation = attachPose.rotation;
    }

    protected virtual Pose CaptureAttachPose(ContainedObject obj) {
        return new Pose {
            position = transform.InverseTransformPoint(obj.GameObject.transform.position),
            rotation = Quaternion.Inverse(transform.rotation) * obj.GameObject.transform.rotation
        };
    }

    public void UnstickObject(GameObject gameObject) {
        if (!Contents.Remove(gameObject, out var obj)) {
            Debug.LogWarning("Tried to remove object to container while already contained.", gameObject);
            return;
        }

        UnstickObject(obj);
    }
    protected void UnstickObject(ContainedObject obj) {
        Debug.Assert(!IsSelecting(obj.Interactable));

        OnUnstickObject(obj);
    }
    protected virtual void OnUnstickObject(ContainedObject obj) {
        Destroy(obj.Joint);
        Destroy(obj.AttachTransform.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(Contents.Count);
            foreach (var (_, obj) in Contents) {
                stream.SendNext(obj.PhotonView.ViewID);
                stream.SendNext(obj.AttachTransform.position);
                stream.SendNext(obj.AttachTransform.rotation);
            }
        } else {
            var currentObjects = Contents.Keys.ToHashSet();

            var count = stream.ReceiveNext<int>();
            for (var i=0; i<count; i++) {
                var interactableViewId = stream.ReceiveNext<int>();
                var attachPosition = stream.ReceiveNext<Vector3>();
                var attachRotation = stream.ReceiveNext<Quaternion>();

                var interactable = PhotonView.Find(interactableViewId).gameObject;
                currentObjects.Remove(interactable);

                ContainedObject obj;
                if (!Contents.TryGetValue(interactable, out obj)) {
                    StickObject(interactable, out obj);
                }
                obj.AttachTransform.localPosition = attachPosition;
                obj.AttachTransform.localRotation = attachRotation;
            }
            foreach (var obj in currentObjects) {
                UnstickObject(obj);
            }
        }
    }

    protected class ContainedObject {
        public GameObject GameObject;
        public Rigidbody Rigidbody;
        public PhotonView PhotonView;
        public XRGrabInteractable Interactable;
        public Transform AttachTransform;
        public FixedJoint Joint;

        public ContainedObject(GameObject gameObject) {
            GameObject = gameObject;
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            PhotonView = gameObject.GetComponent<PhotonView>(); // We use this istead of PhotonView.Get(gameObject) because we want to assert that the view is present in the object itself.
            Interactable = gameObject.GetComponent<XRGrabInteractable>();

            Debug.Assert(Rigidbody != null, $"No {Rigidbody.GetType().Name} on contained object", gameObject);
            Debug.Assert(PhotonView != null, $"No {PhotonView.GetType().Name} on contained object", gameObject);
            Debug.Assert(Interactable != null, $"No {Interactable.GetType().Name} on contained object", gameObject);
        }

        public override bool Equals(object obj) => obj is ContainedObject containedObject && Equals(containedObject);
        private bool Equals(ContainedObject obj) => EqualityComparer<GameObject>.Default.Equals(GameObject, obj.GameObject);
        public override int GetHashCode() => GameObject.GetHashCode();
    }
}
