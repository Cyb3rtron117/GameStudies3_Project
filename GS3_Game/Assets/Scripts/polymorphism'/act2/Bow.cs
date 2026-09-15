using UnityEngine;

public class Bow : Weapon
{
    private string _name;
    private int _damage;
    public override void Attack()
    {
        Debug.Log($"The Bow {_name} attacks with an arrow shot for {_damage} damage!");
    }
    public Bow(string name, int damage) : base(name, damage) 
    {
        _name = name;
        _damage = damage;
    }
}
