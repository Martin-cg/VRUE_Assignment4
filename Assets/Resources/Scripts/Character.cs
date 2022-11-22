using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Character : MonoBehaviourPun, IPunObservable {
    public static Character Local => PhotonNetwork.LocalPlayer.TagObject as Character;
    public bool IsLocal => XRRig != null;

    public XRRig XRRig;
    public GameObject Root;
    public GameObject Head;
    public GameObject LeftHand;
    public GameObject RightHand;

    public int ActorNumber;
    public Lane AssignedLane;

    private void Start() {
        if (IsLocal) {
            foreach (var renderer in Head.GetComponentsInChildren<Renderer>()) {
                renderer.enabled = false;
            }

            SyncTransforms(XRRig.transform, transform);
            SyncTransforms(XRRig.Head.gameObject, Head);
            SyncTransforms(XRRig.LeftHand.gameObject, LeftHand);
            SyncTransforms(XRRig.RightHand.gameObject, RightHand);

            SyncAnimations(XRRig.LeftHand, LeftHand);
            SyncAnimations(XRRig.RightHand, RightHand);
        }
    }
    public void SpawnAtLane(Lane lane) { 
        AssignedLane = lane;
        Root.transform.SetPositionAndRotation(lane.Spawn.transform.position, lane.Spawn.transform.rotation);
        if (XRRig != null) {
            CharacterController CC = XRRig.GetComponent<CharacterController>();
            CC.enabled = false;
            XRRig.transform.SetPositionAndRotation(lane.Spawn.transform.position, lane.Spawn.transform.rotation);
            Physics.SyncTransforms();
            CC.enabled = true;
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(ActorNumber);
        } else {
            ActorNumber = (int) stream.ReceiveNext();
        }
    }
}
