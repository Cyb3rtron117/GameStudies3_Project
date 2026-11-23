using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

public class CinemachineShake : MonoBehaviour
{

    public static CinemachineShake Instance { get; private set; }

    [SerializeField] private List<CinemachineBasicMultiChannelPerlin> CineNoise = new List<CinemachineBasicMultiChannelPerlin>();
    private float shakeTime;
    private float shakeTimeTotal;
    private float startingIntensity;

    [SerializeField] private float GoalIntensity = 1f;
    [SerializeField] private float ShootIntensity = 1f;
    [SerializeField] private float DeadIntensity = 1f;
    [SerializeField] private float BulletIntensity = 1f;

    private void Awake()
    {
        Instance = this;
        CineNoise.Clear();
    }
    public void AddCams(CinemachineCamera playerCam)
    {
        if(!CineNoise.Contains(playerCam.GetComponent<CinemachineBasicMultiChannelPerlin>()))
        {
            CineNoise.Add(playerCam.GetComponent<CinemachineBasicMultiChannelPerlin>());
            playerCam.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0f;
        }
    }
    public void ShakeCam(ShakeType type, float time)
    {
        switch (type)
        {
            case ShakeType.Goal:
                foreach (var noise in CineNoise)
                {
                    noise.AmplitudeGain = GoalIntensity;
                }
                startingIntensity = GoalIntensity;
                break;

            case ShakeType.Shoot:
                foreach (var noise in CineNoise)
                {
                    noise.AmplitudeGain = ShootIntensity;
                }
                startingIntensity = ShootIntensity;
                break;

            case ShakeType.Dead:
                foreach (var noise in CineNoise)
                {
                    noise.AmplitudeGain = DeadIntensity;
                }
                startingIntensity = DeadIntensity;
                break;
            case ShakeType.Ram:
                foreach (var noise in CineNoise)
                {
                    noise.AmplitudeGain = ShootIntensity;
                }
                startingIntensity = ShootIntensity;
                break;
            case ShakeType.BulletHit:
                foreach (var noise in CineNoise)
                {
                    noise.AmplitudeGain = BulletIntensity;
                }
                startingIntensity = BulletIntensity;
                break;

        }
        shakeTime = time;
        shakeTimeTotal = time;
    }

    private void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            foreach (var noise in CineNoise)
            {
                noise.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTime / shakeTimeTotal));
            }
        }
    }
}

public enum ShakeType
{
    Goal,
    Shoot,
    Dead,
    Ram,
    BulletHit
}
