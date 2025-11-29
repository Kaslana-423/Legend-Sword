using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("角色属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;

    [Header("无敌设置")]
    public float invulnerableDuration;
    public bool invulnerable;
    private float invulnerableCounter;

    [Header("组件")]
    private CapsuleCollider2D coll;
    private DataDefinition dataDef; // 【优化】缓存ID组件

    [Header("广播事件")]
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;
    public UnityEvent<Character> OnHealthChange;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        dataDef = GetComponent<DataDefinition>(); // 【优化】Awake缓存，不用每次都Get
    }

    private void OnEnable()
    {
        // 【修复】不要在这里直接 currentHealth = maxHealth; 会覆盖读档数据
        // 如果是全新生成的物体，可以在 Start 里做初始化，或者依赖 newGameEvent

        if (newGameEvent != null)
            newGameEvent.OnEventRaised += NewGame;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        if (newGameEvent != null)
            newGameEvent.OnEventRaised -= NewGame;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
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

    // 注意方法名大小写规范
    void NewGame()
    {
        currentHealth = maxHealth; // 【修复】新游戏必须回血
        currentPower = maxPower;

        // 【修复】复活时，记得把排除的层级加回来（取消排除）
        if (coll != null)
        {
            coll.excludeLayers = LayerMask.GetMask("Nothing"); // 或者恢复为默认
        }

        OnHealthChange?.Invoke(this);
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;

        // 1. 先计算实际扣血
        // 使用 Mathf.Max 防止扣成负数，方便后续逻辑
        float damage = attacker.damage;

        // 2. 只要被打中，无论死没死，都触发受击反馈（击退、音效）
        OnTakeDamage?.Invoke(attacker.transform);

        // 3. 扣血逻辑
        if (currentHealth > damage)
        {
            currentHealth -= damage;
            TriggerInvulnerable();
        }
        else
        {
            currentHealth = 0;

            // 死亡逻辑：排除敌人碰撞
            LayerMask maskToIgnore = LayerMask.GetMask("Enemy");
            // 注意：这里需要确保 coll 确实存在
            if (coll != null)
                coll.excludeLayers = coll.excludeLayers | maskToIgnore;

            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            // 水通常是即死陷阱，即便无敌也得死，或者你可以加 && !invulnerable
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

    public DataDefinition GetDataID()
    {
        return dataDef; // 【优化】直接返回缓存的引用
    }

    public void GetSaveData(Data data)
    {
        // 防御性编程：防止组件丢失
        if (dataDef == null) return;

        string id = dataDef.ID;

        if (data.characterPosDict.ContainsKey(id))
        {
            data.characterPosDict[id] = transform.position;
            data.floatSaveData[id + "health"] = this.currentHealth;
        }
        else
        {
            data.characterPosDict.Add(id, transform.position);
            data.floatSaveData.Add(id + "health", this.currentHealth);
        }
    }

    public void LoadData(Data data)
    {
        if (dataDef == null) return;
        string id = dataDef.ID;

        if (data.characterPosDict.ContainsKey(id))
        {
            transform.position = data.characterPosDict[id];

            // 读取血量
            if (data.floatSaveData.ContainsKey(id + "health"))
            {
                this.currentHealth = data.floatSaveData[id + "health"];
                OnHealthChange?.Invoke(this);

                // 【额外建议】如果读档读出来是死人（血量0），可能需要触发 OnDie
                if (this.currentHealth <= 0)
                {
                    OnDie?.Invoke();
                }
            }
        }
    }
}