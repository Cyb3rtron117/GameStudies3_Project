using UnityEngine;
using UnityEngine.Events;

public class EventsDemo : MonoBehaviour
{
    public bool press = false;
    public UnityEvent OnSpaceBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(press)
        {
            press = false;
            OnSpaceBar.Invoke();
        }
    }
}
