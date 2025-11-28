using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")]
    public VoidEventSO pauseEvent;
    public PlayAudioEventSO BGMEvent;
    public FloatEventSO volumeEvent;
    public PlayAudioEventSO FXEvent;
    [Header("事件广播")]
    public FloatEventSO syncVolumeEvent;
    [Header("参数设置")]
    public AudioSource BGMSourse;
    public AudioSource FXource;
    public AudioMixer mixer;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEVent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        volumeEvent.OnEventRaised += OnVolumeEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;
    }

    private void OnPauseEvent()
    {
        float volume;
        mixer.GetFloat("MasterVolume", out volume);
        syncVolumeEvent.RaiseEvent(volume);
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSourse.clip = clip;
        BGMSourse.Play();
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEVent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        volumeEvent.OnEventRaised -= OnVolumeEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;
    }
    private void OnFXEVent(AudioClip clip)
    {
        FXource.clip = clip;
        FXource.PlayOneShot(clip);
    }
    private void OnVolumeEvent(float volume)
    {
        // 假设你在AudioMixer里有一个参数叫"MasterVolume"
        // Unity的AudioMixer通常用-80到0的dB范围，转换如下
        float dB = volume * 100 - 80;
        mixer.SetFloat("MasterVolume", dB);
    }
}