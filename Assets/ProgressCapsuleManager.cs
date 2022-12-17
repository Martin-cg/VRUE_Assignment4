using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCapsuleManager : MonoBehaviour, IPunObservable
{
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _Progress = 0.0f;
    public float Progress {
        get {
            return _Progress;
        }
        set {
            if (PhotonView.IsMine) {
                _Progress = value;
            }
        }
    }

    private PhotonView PhotonView;

    [SerializeField]
    private GameObject Capsule;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(_Progress);
        } else {
            _Progress = (float)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        Capsule.GetComponent<Renderer>().material.SetFloat("_Progress", Progress);
    }
}
