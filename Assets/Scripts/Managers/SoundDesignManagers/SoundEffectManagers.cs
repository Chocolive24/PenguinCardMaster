using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManagers : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _swordSound;

    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        BaseAttackCard.OnAttackCardPlayed += PlaySwordSound;
    }
    
    private void OnDestroy()
    {
        BaseAttackCard.OnAttackCardPlayed -= PlaySwordSound;
    }

    private void Start()
    {
        
    }

    public void PlaySound(AudioClip sound)
    {
        _audioSource.clip = sound;
        
        _audioSource.Play();
    }
    
    private void PlaySwordSound()
    {
        PlaySound(_swordSound);
    }
}
