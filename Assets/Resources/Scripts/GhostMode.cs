using UnityEngine;

[DisallowMultipleComponent]
public class GhostMode : RoleSpecific {
    public Renderer[] Meshes = new Renderer[0];
    public float Opacity = 0.4f;
    private Material[] OriginalMaterials;
    private Material[] GhostMaterials;

    protected virtual void Reset() {
        RequiredRole = CharacterRole.Spectator;
    }

    protected override void Start() {
        base.Start();

        RequiredRole = CharacterRole.Spectator;
    }

    protected override void OnRoleChanged(CharacterRole role) {
        if (Meshes.Length == 0) {
            Meshes = GetComponentsInChildren<Renderer>();

            GhostMaterials = new Material[Meshes.Length];
        }

        if (OriginalMaterials == null) {
            OriginalMaterials = new Material[Meshes.Length];

            for (var i = 0; i < Meshes.Length; i++) {
                var mesh = Meshes[i];
                OriginalMaterials[i] = mesh.sharedMaterial;
            }
        }

        if (GhostMaterials == null) {
            GhostMaterials = new Material[Meshes.Length];

            for (var i = 0; i < Meshes.Length; i++) {
                var mesh = Meshes[i];
                var material = mesh.material; // this makes a copy
                var color = material.color;
                color.a = Opacity;
                material.color = color;
                // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs
                // we could also create seperate materials at compile time
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                GhostMaterials[i] = material;
            }
        }

        for (var i = 0; i < Meshes.Length; i++) {
            var mesh = Meshes[i];
            mesh.material = role == RequiredRole ? GhostMaterials[i] : OriginalMaterials[i];
        }
    }
}
