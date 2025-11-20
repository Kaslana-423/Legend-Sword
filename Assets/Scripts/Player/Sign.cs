using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    public PlayerInputControl playerInput;
    public Transform playerTrans;
    public GameObject signSprite;
    private bool canPress;
    public IInteractable targetItem;

    private void Awake()
    {
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        playerInput.GamePlay.Confirm.started += OnConfirm;
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    private void Update()
    {
        signSprite.SetActive(canPress);
        signSprite.transform.localScale=playerTrans.localScale;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
            Debug.Log(targetItem.GetType());
        }
        else 
        {
            canPress = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canPress = false;
        }
    }
}
