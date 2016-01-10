using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour 
{
	AudioClip health, power, boost;
    AudioSource sfx, boostAudio;

    void Awake() 
	{
        sfx = GetComponent<AudioSource>();

        //Load Audio Clips
        health = (AudioClip)Resources.Load("Health");
        boost = (AudioClip)Resources.Load("Boost");
        power = (AudioClip)Resources.Load("Power");

        ////Create AudioSources
        boostAudio = gameObject.AddComponent<AudioSource>();
        sfx = gameObject.AddComponent<AudioSource>();

        boostAudio.playOnAwake = false;
        boostAudio.loop = true;
        boostAudio.volume = .01f;
        boostAudio.clip = boost;
    }

    public void Boost(bool play)
    {
        if (play)
        {
            if (!boostAudio.isPlaying)
                boostAudio.Play();
        }

        else
        {
            if (boostAudio.isPlaying)
                boostAudio.Stop();
        }
    }

    public void Health()
    {
        sfx.PlayOneShot(health, 0.2f);
    }

    public void Power()
    {
        sfx.PlayOneShot(power, 0.2f);
    }
}
