using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;
    Transform attacker;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public Vector3 facedir;
    public float hurtForce;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float chaseTime;
    public float chaseTimeCounter;
    public bool chase;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected virtual void Awake() 
    {
        
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    void Update()
    {
        facedir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
    }
    private void FixedUpdate()
    {
        if(!isHurt & !isDead& physicsCheck.isGround)
        Move();
        currentState.PhysicsUpdate();
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(currentSpeed * facedir.x * Time.deltaTime, rb.velocity.y);
    }
    public void TimeCounter()
    {
        if (wait)
        {
            
            waitTimeCounter -= Time.deltaTime;
            if(waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(facedir.x, 1, 1);
                physicsCheck.bottomOffset.x = -physicsCheck.bottomOffset.x;
            }
        }
        if (!FoundPlayer()&& chaseTimeCounter>0)
        {
            chaseTimeCounter-= Time.deltaTime;

        }
        
    }


    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset,
            checkSize, 0, facedir, checkDistance, attackLayer);
    }
    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);   
    }
    #region 事件执行方法
    public void OnTakeDamage(Transform attackTrans)
    {   
        attacker = attackTrans;
        if(attackTrans.position.x-transform.position.x > 0)
        {
            transform.localScale=new Vector3(-1,1,1);
        }
        if(attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        isHurt = true;
        anim.SetTrigger("Hurt");
        Vector2 dir=new Vector2(transform.position.x-attackTrans.position.x,0).normalized;
        rb.velocity = new Vector2(0,rb.velocity.y);
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt=false;
    }

    public void OnDie()
    {
        this.gameObject.layer=2;
        anim.SetBool("Dead",true);
        isDead = true;
    }
    private void OnDisable()
    {
        currentState.OnExit();
    }
    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion  
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset+new Vector3(checkDistance*-transform.localScale.x,0), 0.2f);
    }

}
