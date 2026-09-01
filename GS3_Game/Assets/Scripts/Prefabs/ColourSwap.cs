using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColourSwap : MonoBehaviour
{
    public List<Material> colours = new List<Material>();
    private int index;
    public void SwapColours(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        if (value < 0)//left
        {

        }
        else if (value > 0)//right
        {

        }
    }
}
