using UnityEngine;


[CreateAssetMenu(fileName = "item", menuName = "Items/Default Item")]
public class PickupItem : ScriptableObject
{
    public string itemName;
    public string itemType;
    public Mesh mesh;
    public Material material;
}
[CreateAssetMenu(fileName = "item", menuName = "Items/Special item")]
public class SpecialItem : PickupItem
{
    public string Specialness;
}
