using UnityEngine;

public class TankPrefab : MonoBehaviour
{
    public Transform turret;
    public Transform barrel;
    public SkinnedMeshRenderer renderer;
    public Animator animator;
    public void changeMaterial(Material newMat)
    {
        Material[] materials = renderer.materials;
        materials[0] = newMat;
        renderer.materials = materials;
    }
}
