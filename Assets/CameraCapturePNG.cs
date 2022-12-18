using System.IO;
using UnityEngine;

public class CameraCapturePNG : MonoBehaviour {
    public string ImageName;

    private void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.F9)) {
            CamCapture();
        }
    }

    void CamCapture() {
        Camera Cam = GetComponent<Camera>();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        Debug.Log("Saving image to: " + Application.dataPath + "/Resources/Textures/Recipe/" + ImageName + ".png");
        File.WriteAllBytes(Application.dataPath + "/Resources/Textures/Recipe/" + ImageName + ".png", Bytes);
    }

}
