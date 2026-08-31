using UnityEngine;

public class TankPrefab : MonoBehaviour
{
    public Transform turret;
    public Transform barrel;
    public SkinnedMeshRenderer renderer;
    public Animator animator;
    public BoxCollider collider;
    public float turnSpeed = 10f;
    public float moveSpeed = 10f;
    public float tankweight = 10f;

    public void changeMaterial(Material newMat)
    {
        Material[] materials = renderer.materials;
        materials[0] = newMat;
        renderer.materials = materials;
    }
}
