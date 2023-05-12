using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _mainTheme;
    [SerializeField] private AudioClip _battleTheme;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        BattleManager.OnBattleStart += PlayBattleTheme;
        BattleManager.OnBattleEnd += PlayMainTheme;
    }

    private void OnDestroy()
    {
        BattleManager.OnBattleStart -= PlayBattleTheme;
        BattleManager.OnBattleEnd -= PlayMainTheme;
    }

    private void Start()
    {
        PlayMusic(_mainTheme, 0.3f);
    }

    public void PlayMusic(AudioClip music, float volume)
    {
        _audioSource.clip = music;

        _audioSource.volume = volume;
        
        _audioSource.Play();
    }

    private void PlayBattleTheme(BattleManager arg1, RoomData arg2)
    {
        PlayMusic(_battleTheme, 0.15f);
    }
    
    private void PlayMainTheme(BattleManager arg1, RoomData arg2)
    {
        PlayMusic(_mainTheme, 0.3f);
    }
}
