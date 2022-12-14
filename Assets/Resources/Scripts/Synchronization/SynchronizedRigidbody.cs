using Photon.Pun;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class SynchronizedRigidbody : MonoBehaviourPun, IPunObservable {
    private Rigidbody Rigidbody;

    public bool SyncMass;
    public bool SyncDrag;
    public bool SyncAngularDrag;
    public bool SyncUseGravity;
    public bool SyncIsKinematic;
    public bool SyncInterpolation;
    public bool SyncCollisionDetectionMode;
    public bool SyncConstraints;

    protected virtual void Awake() {
        if (!(SyncMass || SyncDrag || SyncAngularDrag || SyncUseGravity || SyncIsKinematic || SyncInterpolation || SyncCollisionDetectionMode || SyncConstraints)) {
            Destroy(this);
            return;
        }

        Rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            if (SyncMass) {
                stream.SendNext(Rigidbody.mass);
            }
            if (SyncDrag) {
                stream.SendNext(Rigidbody.drag);
            }
            if (SyncAngularDrag) {
                stream.SendNext(Rigidbody.angularDrag);
            }
            if (SyncUseGravity) {
                stream.SendNext(Rigidbody.useGravity);
            }
            if (SyncIsKinematic) {
                stream.SendNext(Rigidbody.isKinematic);
            }
            if (SyncInterpolation) {
                stream.SendNext(Rigidbody.interpolation);
            }
            if (SyncCollisionDetectionMode) {
                stream.SendNext(Rigidbody.collisionDetectionMode);
            }
            if (SyncConstraints) {
                stream.SendNext(Rigidbody.constraints);
            }
        } else {
            if (SyncMass) {
                Rigidbody.mass = stream.ReceiveNext<float>();
            }
            if (SyncDrag) {
                Rigidbody.drag = stream.ReceiveNext<float>();
            }
            if (SyncAngularDrag) {
                Rigidbody.angularDrag = stream.ReceiveNext<float>();
            }
            if (SyncUseGravity) {
                Rigidbody.useGravity = stream.ReceiveNext<bool>();
            }
            if (SyncIsKinematic) {
                Rigidbody.isKinematic = stream.ReceiveNext<bool>();
            }
            if (SyncInterpolation) {
                Rigidbody.interpolation = stream.ReceiveNext<RigidbodyInterpolation>();
            }
            if (SyncCollisionDetectionMode) {
                Rigidbody.collisionDetectionMode = stream.ReceiveNext<CollisionDetectionMode>();
            }
            if (SyncConstraints) {
                Rigidbody.constraints = stream.ReceiveNext<RigidbodyConstraints>();
            }
        }
    }
}
