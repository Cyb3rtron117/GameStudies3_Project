using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public AudioSource BigBoom;
    public AudioSource Tankidle;
    public List<AudioSource> SmashSounds = new List<AudioSource>();
    public List<AudioSource> MgSounds = new List<AudioSource>();
    public List<AudioSource> ShootSounds = new List<AudioSource>();

    public void PlayBoom()
    {
        if(!BigBoom.isPlaying)
        {
            BigBoom.pitch = Random.Range(0.7f, 1.1f);
            BigBoom.Play();
        }
    }
    public void PlayIdle()
    {
        if (!Tankidle.isPlaying)
        {
            Tankidle.Play();
        }
    }
    public void PlayMG()
    {
        if (!MgSounds[0].isPlaying)
        {
            AudioSource sound = MgSounds[0];
            MgSounds.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            MgSounds.Add(sound);
        }
    }
    public void PlayShoot()
    {
        if (!ShootSounds[0].isPlaying)
        {
            AudioSource sound = ShootSounds[0];
            ShootSounds.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            ShootSounds.Add(sound);
        }
    }
    public void PlaySmash()
    {
        if (!SmashSounds[0].isPlaying)
        {
            AudioSource sound = SmashSounds[0];
            SmashSounds.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            SmashSounds.Add(sound);
        }
    }
}
