using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")]
    public PlayAudioEventSO BGMEvent;
    public PlayAudioEventSO FXEvent;
    [Header("组件")]
    public AudioSource BGMSourse;
    public AudioSource FXource;
 
    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEVent;
        BGMEvent.OnEventRaised += OnBGMEvent;
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSourse.clip = clip;
        BGMSourse.Play();
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEVent;
    }
    private void OnFXEVent(AudioClip clip)
    {
        FXource.clip = clip;
        FXource.PlayOneShot(clip);
    }
}