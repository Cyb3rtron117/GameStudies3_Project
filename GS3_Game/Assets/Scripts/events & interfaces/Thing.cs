using UnityEngine;

public class Thing : MonoBehaviour, IPickupable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pickup()
    {
        Debug.Log("Picked up thing");
    }
}
