using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;
    public UnityEvent<Character> OnHealthChange;
    void newGame()
    {
        
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }
    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        Debug.Log(maxHealth);
        Debug.Log(this.transform.name);
        currentHealth = maxHealth;
        newGameEvent.OnEventRaised += newGame;
        
    }
    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= newGame;
    }
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable) 
            return;
        if (currentHealth > attacker.damage)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //执行受伤 用事件
            OnTakeDamage?.Invoke(attacker.transform);
            
        }
        else
        {
            currentHealth = 0;
            //死亡
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.CompareTag("Water"))
        {
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
           OnDie?.Invoke();
        }
    }
    void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable= true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if(invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }
}
