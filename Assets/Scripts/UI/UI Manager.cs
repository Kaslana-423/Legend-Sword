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
    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    public GameObject moblieTouch;
    public Button SettingBtn;
    public GameObject pauseBtn;
    private void Awake()
    {
#if UNITY_STANDALONE
         moblieTouch.SetActive(false);
#endif
    }
    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvnet;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvnet;
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
