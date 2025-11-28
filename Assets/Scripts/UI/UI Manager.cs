using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public PlayerStatBar playerStatBar;
    public VoidEventSO gameOverEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;
    [Header("广播事件")]
    public VoidEventSO pauseEvent;
    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    public GameObject moblieTouch;
    public Button SettingBtn;
    public GameObject pauseBtn;
    public Slider volumeSlider;
    private void Awake()
    {
#if UNITY_STANDALONE
        moblieTouch.SetActive(false);
#endif
        SettingBtn.onClick.AddListener(TogglePausePanel);
    }
    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvnet;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvnet;
        syncVolumeEvent.OnEventRaised += OnsyncVolumeEvent;
    }

    private void OnsyncVolumeEvent(float arg0)
    {
        volumeSlider.value = (arg0 + 80) / 100;
    }

    private void OnLoadDataEvnet()
    {
        gameOverPanel.SetActive(false);
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvnet;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvnet;
        syncVolumeEvent.OnEventRaised -= OnsyncVolumeEvent;
    }

    private void TogglePausePanel()
    {
        bool isActive = pauseBtn.activeSelf;

        if (!isActive)
        {
            pauseEvent.RaiseEvent();
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        pauseBtn.SetActive(!isActive);
    }
    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(persentage);
    }
}
