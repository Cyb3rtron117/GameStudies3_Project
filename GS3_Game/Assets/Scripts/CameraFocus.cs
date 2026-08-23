using UnityEngine;
using Unity.Cinemachine;

public class CameraFocus : MonoBehaviour
{
    public CinemachineBrain Brain;
    public ICinemachineCamera CamA;
    public ICinemachineCamera CamB;
    private void Start()
    {
        CamA = GetComponent<CinemachineCamera>();
        CamB = GetComponent<CinemachineCamera>();

        //Override parameters
        int layer = 1; //Layer to override
        int priority = 1;
        float weight = 1f;
        float blendTime = 0f;
        Brain.SetCameraOverride(layer, priority, CamA, CamB, weight, blendTime);
    }
}
