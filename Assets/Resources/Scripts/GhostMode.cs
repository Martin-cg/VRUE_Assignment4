using UnityEngine;

[DisallowMultipleComponent]
public class GhostMode : RoleSpecific {
    public Renderer[] Meshes = new Renderer[0];
    private Material[] OriginalMaterials;
    public Material GhostMaterial;

    protected virtual void Reset() {
        RequiredRole = CharacterRole.Spectator;
    }

    protected override void OnRoleChanged(bool isRequiredRole) {
        if (Meshes.Length == 0) {
            Meshes = GetComponentsInChildren<Renderer>();
        }

        if (OriginalMaterials == null) {
            OriginalMaterials = new Material[Meshes.Length];

            for (var i = 0; i < Meshes.Length; i++) {
                var mesh = Meshes[i];
                OriginalMaterials[i] = mesh.sharedMaterial;
            }
        }

        for (var i = 0; i < Meshes.Length; i++) {
            var mesh = Meshes[i];
            mesh.material = isRequiredRole ? GhostMaterial : OriginalMaterials[i];
        }
    }
}
