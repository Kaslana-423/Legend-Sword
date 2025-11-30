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
    public bool isDead;

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
            isDead = true;
            if (DataManager.instance != null)
            {
                DataManager.instance.SaveOneObject(this);
                Debug.Log("Character " + dataDef.ID + " marked as dead in DataManager.");
            }
            Debug.Log(this.gameObject.name + " died.");
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
        return dataDef;
    }


    public void GetSaveData(Data data)
    {
        if (dataDef == null) return;
        string id = dataDef.ID;

        // 1. 保存位置
        if (data.characterPosDict.ContainsKey(id))
            data.characterPosDict[id] = transform.position;
        else
            data.characterPosDict.Add(id, transform.position);

        // 2. 保存血量
        string healthKey = id + "health";
        if (data.floatSaveData.ContainsKey(healthKey))
            data.floatSaveData[healthKey] = this.currentHealth;
        else
            data.floatSaveData.Add(healthKey, this.currentHealth);

        // 3. 保存死亡状态
        string boolKey = id + "isDead";
        // 【调试日志】看看这一步到底有没有执行，以及存了什么
        Debug.Log($"保存对象: {this.gameObject.name} | ID: {id} | isDead: {isDead}");

        if (data.boolSaveData.ContainsKey(boolKey))
            data.boolSaveData[boolKey] = isDead;
        else
            data.boolSaveData.Add(boolKey, isDead);
    }

    public void LoadData(Data data)
    {
        if (dataDef == null) return;
        string id = dataDef.ID;

        // 只有位置存在时才开始读取
        if (data.characterPosDict.ContainsKey(id))
        {
            // 1. 安全读取死亡状态
            string boolKey = id + "isDead";

            // 默认认为是活的
            bool isDeadInSave = false;

            // 【关键修复】先检查有没有这个 Key，防止报错
            if (data.boolSaveData.ContainsKey(boolKey))
            {
                isDeadInSave = data.boolSaveData[boolKey];
            }

            // 2. 根据状态处理
            if (isDeadInSave)
            {
                this.gameObject.SetActive(false);
                Debug.Log("2222");
                // 如果是死人，直接结束，不用同步位置和血量了
                return;

            }
            else
            {
                this.gameObject.SetActive(true);
            }

            // 3. 如果没死，同步位置和血量
            transform.position = data.characterPosDict[id];

            string healthKey = id + "health";
            if (data.floatSaveData.ContainsKey(healthKey))
            {
                this.currentHealth = data.floatSaveData[healthKey];
                OnHealthChange?.Invoke(this);
            }
        }
    }
}