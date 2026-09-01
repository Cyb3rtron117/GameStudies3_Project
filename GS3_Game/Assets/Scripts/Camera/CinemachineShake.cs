using Unity.Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{

    public static CinemachineShake Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin CineNoise;
    private float shakeTime;
    private float shakeTimeTotal;
    private float startingIntensity;

    private void Awake()
    {
        Instance = this;
        CineNoise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        CineNoise.AmplitudeGain = 0f;
    }

    public void shakeCam(float intensity, float time)
    {
        CineNoise.AmplitudeGain = intensity;
        shakeTime = time;
        shakeTimeTotal = time;
        startingIntensity = intensity;
    }

    private void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            CineNoise.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTime / shakeTimeTotal));
        }
    }
}
