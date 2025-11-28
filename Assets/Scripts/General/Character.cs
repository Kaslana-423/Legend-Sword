using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    [Header("�¼�����")]
    public VoidEventSO newGameEvent;
    [Header("��������")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    [Header("�����޵�")]
    public float invulnerableDuration;
    private CapsuleCollider2D coll;
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
        coll = GetComponent<CapsuleCollider2D>();
    }
    private void OnEnable()
    {

        currentHealth = maxHealth;
        newGameEvent.OnEventRaised += newGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();

    }
    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= newGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;
        if (currentHealth > attacker.damage)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //ִ������ ���¼�
            OnTakeDamage?.Invoke(attacker.transform);

        }
        else
        {
            currentHealth = 0;
            LayerMask maskToIgnore = LayerMask.GetMask("Enemy");
            coll.excludeLayers = coll.excludeLayers | maskToIgnore;
            //����
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie?.Invoke();
            }

        }
    }
    void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = transform.position;
            data.floatSaveData[GetDataID().ID + "health"] = this.currentHealth;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, transform.position);
            data.floatSaveData.Add(GetDataID().ID + "health", this.currentHealth);
        }
    }

    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID];
            this.currentHealth = data.floatSaveData[GetDataID().ID + "health"];
            OnHealthChange?.Invoke(this);
        }
    }
}
