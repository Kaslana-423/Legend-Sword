using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    } 
    void Start()
    {
       
    }
     
   
    void Update()
    {
        SetAnimation();
    }
    public void SetAnimation()
    {
        anim.SetFloat("velocityX",Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isGround",physicsCheck.isGround);
        anim.SetBool("isDeath", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
    }
    public void PlayHurt()
    {
        anim.SetTrigger("Hurt");
    }
    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }
    
}
