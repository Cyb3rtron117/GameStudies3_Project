using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "tank", menuName = "TankPrefab")]
public class TankObject : ScriptableObject
{
    public Transform turret;
    public Transform barrel;
    public SkinnedMeshRenderer renderer;
    public AnimatorController animator;

    [Header("Box Collider")]
    public Vector3 BoxCenter;
    public Vector3 BoxSize;

    [Header("Handling")]
    public float turnSpeed;
    public float moveSpeed;
    public float tankweight;
}
