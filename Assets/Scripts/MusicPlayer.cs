using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource player;

    private void Awake()
    {
        player = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!player.isPlaying)
        {
            if (player == null)
            {
                Debug.LogWarning("No audio source component");
                return;
            }

            player.clip = clips[Random.Range(0, clips.Length)];
            player.PlayDelayed(0.25f);
        }
    }
}
