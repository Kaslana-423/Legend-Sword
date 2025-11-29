using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float damageRate;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 先尝试获取组件，存到一个临时变量里
        Character character = collision.GetComponent<Character>();

        // 2. 【安全检查】先看它是不是 null
        // 如果碰到的是墙壁、地板，character 就是 null，直接 return 跳过
        if (character == null) return;

        // 3. 既然不是 null，说明碰到的是活物，再检查是不是尸体
        // 逻辑锁：如果已经死了，就不再鞭尸
        if (character.currentHealth <= 0) return;

        // 4. 一切正常，造成伤害
        character.TakeDamage(this);
    }
    void Start()
    {

    }


    void Update()
    {

    }
}
