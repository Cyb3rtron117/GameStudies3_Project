using UnityEngine;

public class Sword : Weapon
{
    private string _name;
    private int _damage;
    public override void Attack()
    {
        Debug.Log($"sword {_name} attacks with a slash for {_damage} damage!");
    }
    public Sword(string name, int damage) : base(name, damage)
    {
        _name = name;
        _damage = damage;
    }
}
