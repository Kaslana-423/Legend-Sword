using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy 
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
    }

}
