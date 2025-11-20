using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
public class CameraCtrol : MonoBehaviour
{
    public VoidEventSO afterSceneLoadEvent;
    public VoidEventSO cameraShakeEvent;
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadEvent.OnEventRaised += OnAfterSceneLoad;
    }


    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadEvent.OnEventRaised -= OnAfterSceneLoad;
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null) return;
        else
        {
            confiner2D.m_BoundingShape2D=obj.GetComponent<Collider2D>();
        }
        confiner2D.InvalidateCache();
    }
    private void OnAfterSceneLoad()
    {
        GetNewCameraBounds();
    }
}
