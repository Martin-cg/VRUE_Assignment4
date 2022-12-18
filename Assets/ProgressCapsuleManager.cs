using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCapsuleManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _Progress = 0.0f;
    public float Progress {
        get {
            return _Progress;
        }
        set {
            _Progress = value;
        }
    }

    //private PhotonView PhotonView;

    [SerializeField]
    private GameObject Capsule;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Capsule.GetComponent<Renderer>().material.SetFloat("_Progress", Progress);
    }
}
