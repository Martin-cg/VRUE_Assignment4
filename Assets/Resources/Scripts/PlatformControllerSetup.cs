using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#else
using UnityEngine.XR.Management;
#endif


public class PlatformControllerSetup : MonoBehaviour {
    public GameObject DefaultController;
    public GameObject OculusController;

    public void Start() {
#if UNITY_EDITOR
        var loaders = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone).Manager.activeLoaders;
#else
        var loaders = XRGeneralSettings.Instance.Manager.activeLoaders;
#endif

        if (loaders.Any(loader => loader.name == "Oculus Loader")) {
            DefaultController.SetActive(false);
            OculusController.SetActive(true);
        }
    }
}
