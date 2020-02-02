using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioSource break1;
    public AudioSource break2;
    public AudioSource repair1;
    public AudioSource repair2;
    public AudioSource explo1;
    public AudioSource explo2;
    public AudioSource pickup1;
    public AudioSource pickup2;
    public AudioSource throw1;
    public AudioSource throw2;
    public AudioSource fall1;
    public AudioSource fall2;
    public AudioSource pickupSpawn;
    public AudioSource menuClick;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void BreakSound()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = break1;
        if (break1.isPlaying)
        {
            s = break2;
        }
        s.pitch = pitch;
        s.Play();
    }

    public void RepairSound()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = repair1;
        if (repair1.isPlaying)
        {
            s = repair2;
        }
        s.pitch = pitch;
        s.Play();
    }

    public void ExplosionSound()
    {
        print("Boom");
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = explo1;
        if (explo1.isPlaying)
        {
            s = explo2;
        }
        s.pitch = pitch;
        s.Play();
    }

    public void PickupSound()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = pickup1;
        if (pickup1.isPlaying)
        {
            s = pickup2;
        }
        s.pitch = pitch;
        s.Play();
    }

    public void ThrowSound()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = throw1;
        if (throw1.isPlaying)
        {
            s = throw2;
        }
        s.pitch = pitch;
        s.Play();
    }

    public void FallSound()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = fall1;
        if (fall1.isPlaying)
        {
            s = fall2;
        }
        s.pitch = pitch;
        s.Play();
    }

    public void PickupSpawnSound()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        AudioSource s = pickupSpawn;
        s.pitch = pitch;
        s.Play();
    }

    public void MenuClickSound()
    {
        menuClick.Play();
    }
}
