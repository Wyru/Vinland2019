using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterBase))]
public class CharacterSounds : MonoBehaviour
{
    [Header("AudioClips")]
    public AudioClip walk;
    public AudioClip jump;
    public AudioClip attack;
    public AudioClip damage;
    public AudioClip evade;
    public AudioClip die;

    [Header("Delays")]
    [Range(0,1)]
    public float attackDelay;

    private AudioSource audioSource;
    private CharacterBase cb;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cb = GetComponent<CharacterBase>();

        if (walk != null)
            cb.OnCharacterWalk += playWalkSound;

        if (walk != jump)
            cb.OnCharacterJump += playJumpSound;

        if (attack != null)
            cb.OnCharacterAttack += playAttackSound;

        if (damage != null)
            cb.OnCharacterTakeDamage += playTakeDamageSound;

        if (evade != null)
            cb.OnCharacterEvade += playEvadeSound;

        if (die != null)
            cb.OnCharacterDie += playDieSound;

    }

    void playWalkSound()
    {
        audioSource.PlayOneShot(walk);
    }

    void playAttackSound()
    {
        StartCoroutine(playSound(attack,attackDelay));
    }

    void playJumpSound()
    {
        audioSource.PlayOneShot(jump);
    }

    void playEvadeSound()
    {
        audioSource.PlayOneShot(evade);
    }

    void playTakeDamageSound()
    {
        audioSource.PlayOneShot(damage);
    }

    void playDieSound()
    {
        audioSource.PlayOneShot(die);
    }


    IEnumerator playSound(AudioClip a, float delay){
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(a);
    }
}
