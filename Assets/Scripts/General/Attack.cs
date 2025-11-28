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

        collision.GetComponent<Character>()?.TakeDamage(this);
    }
    void Start()
    {

    }


    void Update()
    {

    }
}
