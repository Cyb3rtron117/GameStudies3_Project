using UnityEngine;
using System.Collections.Generic;

public class Action2 : MonoBehaviour
{
    private List<Weapon> weapons = new List<Weapon>();
    void Start()
    {
        Sword sword = new Sword("Excalibur", 99);
        weapons.Add(sword);
        Bow bow = new Bow("Wooden Bow", 1);
        weapons.Add(bow);

        foreach(Weapon weapon in weapons)
        {
            weapon.Attack();
        }
    }
}
