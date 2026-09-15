using UnityEngine;

public class Weapon
{
    public string Name;
    public int Damage;

    public virtual void Attack()
    {
        Debug.Log("weapon is attacking");
    }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }
}
