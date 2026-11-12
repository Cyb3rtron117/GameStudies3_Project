using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ViewportFix : MonoBehaviour
{
    [SerializeField] Camera baseCamera;
    [SerializeField] Camera overlayCamera;
    private void Start()
    {
        overlayCamera.rect = baseCamera.rect;
    }
}
