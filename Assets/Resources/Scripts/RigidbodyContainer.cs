using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class RigidbodyContainer : XRSocketInteractor, IPunObservable {
    public float RestickDelay = 1;

    public Rigidbody Rigidbody;
    public PhotonView PhotonView;
    protected readonly IDictionary<GameObject, ContainedObject> Contents = new Dictionary<GameObject, ContainedObject>();
    protected readonly IDictionary<GameObject, ContainedObject> HoveredContents = new Dictionary<GameObject, ContainedObject>();
    private readonly ISet<IXRInteractable> BlockedInteractables = new HashSet<IXRInteractable>();

    protected override void Awake() {
        base.Awake();

        Rigidbody = GetComponent<Rigidbody>();
        PhotonView = GetComponent<PhotonView>();

        if (GetComponent<XRBaseInteractable>()) {
            gameObject.GetOrAddComponent<SynchronizedMoveableSocketInteractor>();
        }
    }

    public override Transform GetAttachTransform(IXRInteractable interactable) {
        return Contents.TryGetValue(interactable.transform.gameObject, out var obj) ? obj.AttachTransform : base.GetAttachTransform(interactable);
    }

    public override bool CanHover(IXRHoverInteractable interactable) {
        return base.CanHover(interactable) && interactable.transform.gameObject.HasComponent<Ingredient>();
    }
    public override bool CanSelect(IXRSelectInteractable interactable) {
        // TODO: maybe limit to interactables that have been recently touched.
        return (!interactable.isSelected || (IsSelecting(interactable) && interactable.interactorsSelecting.Count == 1))
            && interactable.transform.gameObject.HasComponent<Ingredient>()
            && !BlockedInteractables.Contains(interactable);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        StickObject(args.interactableObject.transform.gameObject);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);

        UnstickObject(args.interactableObject.transform.gameObject);
    }

    public void StickObject(GameObject gameObject) {
        if (Contents.ContainsKey(gameObject)) {
            return;
        }

        if (PhotonView.IsMine) {
            DoStickObject(gameObject);
        } else {
            PhotonView.RPC(nameof(AskStickObject), RpcTarget.Others, PhotonView.Get(gameObject).ViewID);
        }
    }
    public void DoStickObject(GameObject gameObject) {
        DoStickObject(gameObject, out var _);
    }
    protected void DoStickObject(GameObject gameObject, out ContainedObject containedObject) {
        var obj = new ContainedObject(gameObject) {
            Rigidbody = gameObject.GetComponent<Rigidbody>(),
            PhotonView = gameObject.GetComponent<PhotonView>(), // We use this istead of PhotonView.Get(gameObject) because we want to assert that the view is present in the object itself.
            Interactable = gameObject.GetComponent<XRGrabInteractable>(),
            AttachTransform = new GameObject($"[Attach] {gameObject.name}").transform
        };
        obj.AttachTransform.parent = transform;

        Debug.Assert(obj.Rigidbody != null, $"No {obj.Rigidbody.GetType().Name} on contained object", gameObject);
        Debug.Assert(obj.PhotonView != null, $"No {obj.PhotonView.GetType().Name} on contained object", gameObject);
        Debug.Assert(obj.Interactable != null, $"No {obj.Interactable.GetType().Name} on contained object", gameObject);

        var added = Contents.TryAdd(gameObject, obj);
        if (!added) {
            Debug.LogWarning("Tried to add object to container while already contained.", gameObject);
            Destroy(obj.AttachTransform.gameObject);
            containedObject = Contents[gameObject];
            return;
        }

        OnStickObject(obj);
        containedObject = obj;
    }

    protected virtual void OnStickObject(ContainedObject obj) {
        if (!IsSelecting(obj.Interactable)) {
            interactionManager.SelectEnter(this, obj.Interactable as IXRSelectInteractable);
        }

        obj.Rigidbody.gameObject.SetLayerRecursively(Layers.IngredientsOnPlate);

        var attachPose = CaptureAttachPose(obj);
        obj.AttachTransform.SetLocalPose(attachPose);
    }

    protected virtual Pose CaptureAttachPose(ContainedObject obj) {
        return new Pose {
            position = transform.InverseTransformPoint(obj.GameObject.transform.position),
            rotation = Quaternion.Inverse(transform.rotation) * obj.GameObject.transform.rotation
        };
    }

    public void UnstickObject(GameObject gameObject) {
        if (!Contents.ContainsKey(gameObject)) {
            return;
        }

        if (PhotonView.IsMine) {
            DoUnstickObject(gameObject);
        } else {
            PhotonView.RPC(nameof(AskUnstickObject), RpcTarget.Others, PhotonView.Get(gameObject).ViewID);
        }
    }
    public void DoUnstickObject(GameObject gameObject) {
        if (!Contents.Remove(gameObject, out var obj)) {
            Debug.LogWarning("Tried to remove object to container while already contained.", gameObject);
            return;
        }

        DoUnstickObject(obj);
    }
    protected void DoUnstickObject(ContainedObject obj) {
        OnUnstickObject(obj);
    }
    protected virtual void OnUnstickObject(ContainedObject obj) {
        if (IsSelecting(obj.Interactable)) {
            interactionManager.SelectExit(this, obj.Interactable as IXRSelectInteractable);
        }

        Destroy(obj.AttachTransform.gameObject);

        obj.Rigidbody.isKinematic = false;
        obj.Rigidbody.useGravity = true;

        BlockedInteractables.Add(obj.Interactable);
        HoveredContents.TryAdd(obj.GameObject, obj);
        if (gameObject && gameObject.activeInHierarchy) {
            StartCoroutine(ResetInteractableDelayed());
        }

        IEnumerator ResetInteractableDelayed() {
            yield return new WaitForSeconds(1f);
            ResetInteractable(obj.Interactable);
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args) {
        base.OnHoverExited(args);

        ResetInteractable(args.interactableObject);
    }

    private void ResetInteractable(IXRInteractable interactable) {
        BlockedInteractables.Remove(interactable);
        if (HoveredContents.Remove(interactable.transform.gameObject, out var obj)) {
            if (obj.Rigidbody.gameObject.layer != Layers.IngredientsOnPlate) {
                return;
            }

            obj.Rigidbody.gameObject.SetLayerRecursively(Layers.Ingredients);
        }
    }

    [PunRPC]
    public void AskStickObject(int viewId) {
        if (PhotonView.IsMine) {
            var interactable = PhotonView.Find(viewId).gameObject;
            StickObject(interactable);
        }
    }

    [PunRPC]
    public void AskUnstickObject(int viewId) {
        if (PhotonView.IsMine) {
            var interactable = PhotonView.Find(viewId).gameObject;
            UnstickObject(interactable);
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(Contents.Count);
            foreach (var (_, obj) in Contents) {
                stream.SendNext(obj.PhotonView.ViewID);
                stream.SendNext(obj.AttachTransform.localPosition);
                stream.SendNext(obj.AttachTransform.localRotation);
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

                if (!Contents.TryGetValue(interactable, out var obj)) {
                    DoStickObject(interactable, out obj);
                }
                obj.AttachTransform.localPosition = attachPosition;
                obj.AttachTransform.localRotation = attachRotation;
            }

            foreach (var obj in currentObjects) {
                DoUnstickObject(obj);
            }
        }
    }

    protected class ContainedObject {
        public GameObject GameObject;
        public Rigidbody Rigidbody;
        public PhotonView PhotonView;
        public XRGrabInteractable Interactable;
        public Transform AttachTransform;

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
