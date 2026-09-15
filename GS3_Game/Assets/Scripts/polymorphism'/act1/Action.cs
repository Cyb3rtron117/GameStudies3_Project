using UnityEngine;

public class Action : MonoBehaviour
{
    public string characterName;
    public string weaponName;
    public Warrior warrior;
    void Start()
    {
        warrior = new Warrior(characterName, weaponName);
        print(warrior.Name);
        print(warrior.Weapon);
    }
}
