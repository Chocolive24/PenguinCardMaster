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
        PlayMusic(_mainTheme);
    }

    public void PlayMusic(AudioClip music)
    {
        _audioSource.clip = music;
        
        _audioSource.Play();
    }

    private void PlayBattleTheme(BattleManager arg1, RoomData arg2)
    {
        PlayMusic(_battleTheme);
    }
    
    private void PlayMainTheme(BattleManager arg1, RoomData arg2)
    {
        PlayMusic(_mainTheme);
    }
}
