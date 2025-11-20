using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.chaseTimeCounter = currentEnemy.chaseTime;
        }
        if(currentEnemy.chaseTimeCounter<=0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
            
        }

        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.isWallLeft && currentEnemy.facedir.x < 0)|| (currentEnemy.physicsCheck.isWallRight && currentEnemy.facedir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.facedir.x, 1, 1);
        }
    }

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("Run",true);
        currentEnemy.chase = true;
        currentEnemy.chaseTimeCounter = currentEnemy.chaseTime;
    }

    public override void OnExit()
    {
        currentEnemy.chaseTimeCounter -= currentEnemy.chaseTime;
        currentEnemy.anim.SetBool("Run",false);
    }

    public override void PhysicsUpdate()
    {
      
    }
}
