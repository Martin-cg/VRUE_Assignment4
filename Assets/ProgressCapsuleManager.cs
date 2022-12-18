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

    public Color ProgressColor;

    private Material ThisMaterial;

    // Start is called before the first frame update
    void Start()
    {
        ThisMaterial = Capsule.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        ThisMaterial.SetFloat("_Progress", Progress);
        ThisMaterial.SetColor("_Fill_Color", ProgressColor);
    }
}
