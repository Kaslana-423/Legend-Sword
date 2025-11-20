using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        
        if (!currentEnemy.physicsCheck.isGround||currentEnemy.physicsCheck.isWallLeft && currentEnemy.facedir.x < 0)
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("Walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("Walk", true);
        }
        if (!currentEnemy.physicsCheck.isGround || currentEnemy.physicsCheck.isWallRight && currentEnemy.facedir.x > 0)
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("Walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("Walk", true);
        }
    }

    public override void OnEnter(Enemy enemy)
    {
       currentEnemy = enemy;
       currentEnemy.currentSpeed=currentEnemy.normalSpeed;
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("Walk",false);
        Debug.Log("Exit");
    }

    public override void PhysicsUpdate()
    {
       
    }
}
