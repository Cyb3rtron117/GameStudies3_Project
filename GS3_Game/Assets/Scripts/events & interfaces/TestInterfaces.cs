using UnityEngine;
using System.Collections.Generic;

public class TestInterfaces : MonoBehaviour
{
    public GameObject[] objects;
    void Start()
    {
        foreach(var obj in objects)
        {
            if(obj.GetComponent<IPickupable>() != null)
            {
                obj.GetComponent<IPickupable>().Pickup();
            }
        }
    }
}
