using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Player))]
public class BerserkSounds : MonoBehaviour
{
    [Header("AudioClips")]
    public AudioClip awakening;

    private AudioSource audioSource;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GetComponent<Player>();
        if(awakening != null){
            player.OnPlayerBerserk += playAwakeningSound;
        }
    }

    void playAwakeningSound()
    {
        audioSource.PlayOneShot(awakening);
    }
}
