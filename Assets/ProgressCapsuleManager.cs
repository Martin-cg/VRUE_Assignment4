using System.Diagnostics;
using UnityEngine;

public class ProgressCapsuleManager : MonoBehaviour {
    public long HideTimeoutMS = 2000;
    private readonly Stopwatch TimeSinceLastPropertyChange = new();

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _Progress = 0.0f;
    public float Progress {
        get {
            return _Progress;
        }
        set {
            if (value != _Progress) {
                TimeSinceLastPropertyChange.Restart();
                _Progress = value;

                UpdateMaterial();
            }
        }
    }

    //private PhotonView PhotonView;

    [SerializeField]
    private GameObject Capsule;

    [SerializeField]
    private Color _ProgressColor;

    public Color ProgressColor {
        get {
            return _ProgressColor;
        }
        set {
            if (value != _ProgressColor) {
                TimeSinceLastPropertyChange.Restart();
                _ProgressColor = value;

                UpdateMaterial();
            }
        }
    }

    private Material ThisMaterial;

    // Start is called before the first frame update
    void Awake() {
        if (ThisMaterial == null) {
            ThisMaterial = Capsule.GetComponent<Renderer>().material;
        }
    }

    // Update is called once per frame
    void Update() {
        if (TimeSinceLastPropertyChange.ElapsedMilliseconds >= HideTimeoutMS) {
            ThisMaterial.SetFloat("_Hide", 1.0F);
        } else {
            ThisMaterial.SetFloat("_Hide", 0.0F);
        }
    }

    private void UpdateMaterial() {
        ThisMaterial.SetFloat("_Progress", _Progress);
        ThisMaterial.SetColor("_Fill_Color", _ProgressColor);
    }
}
