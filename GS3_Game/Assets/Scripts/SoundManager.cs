using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public AudioSource BigBoom;
    public AudioSource Tankidle;
    public AudioSource MissileImpact;
    public AudioSource Reroll;
    public List<AudioSource> SmashSounds = new List<AudioSource>();
    public List<AudioSource> playerExplode = new List<AudioSource>();
    public List<AudioSource> MgImpactSounds = new List<AudioSource>();
    public List<AudioSource> ShootSounds = new List<AudioSource>();
    public List<AudioSource> UIClicks = new List<AudioSource>();
    public List<AudioSource> UIErrors = new List<AudioSource>();
    public List<AudioSource> Wooshes = new List<AudioSource>();
    public List<AudioSource> Respawns = new List<AudioSource>();

    public void PlayBoom()
    {
        if(!BigBoom.isPlaying)
        {
            BigBoom.pitch = Random.Range(0.7f, 1.1f);
            BigBoom.Play();
        }
    }
    public void PlayMissileImp()
    {
        if (!MissileImpact.isPlaying)
        {
            MissileImpact.pitch = Random.Range(0.7f, 1.1f);
            MissileImpact.Play();
        }
    }
    public void PlayIdle()
    {
        if (!Tankidle.isPlaying)
        {
            Tankidle.Play();
        }
    }
    public void PlayReroll()
    {
        if (!Reroll.isPlaying)
        {
            Reroll.Play();
        }
    }
    public void PlayPlayerExpl()
    {
        if (!playerExplode[0].isPlaying)
        {
            AudioSource sound = playerExplode[0];
            playerExplode.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            playerExplode.Add(sound);
        }
    }
    public void PlayPing()
    {
        if (!MgImpactSounds[0].isPlaying)
        {
            AudioSource sound = MgImpactSounds[0];
            MgImpactSounds.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            MgImpactSounds.Add(sound);
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

    public void PlayClick()
    {
        if (!UIClicks[0].isPlaying)
        {
            AudioSource sound = UIClicks[0];
            UIClicks.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            UIClicks.Add(sound);
        }
    }
    public void PlayError()
    {
        if (!UIErrors[0].isPlaying)
        {
            AudioSource sound = UIErrors[0];
            UIErrors.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            UIErrors.Add(sound);
        }
    }
    public void PlayWoosh()
    {
        if (!Wooshes[0].isPlaying)
        {
            AudioSource sound = Wooshes[0];
            Wooshes.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            Wooshes.Add(sound);
        }
    }
    public void PlayRespawn()
    {
        if (!Respawns[0].isPlaying)
        {
            AudioSource sound = Respawns[0];
            Respawns.Remove(sound);
            sound.pitch = Random.Range(0.7f, 1.1f);
            sound.Play();
            Respawns.Add(sound);
        }
    }
}
