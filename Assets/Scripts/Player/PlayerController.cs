using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public VoidEventSO backToMenuEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public SceneLoadEventSO sceneLoadEvent;
    [Header("基本参数")]
    public Vector2 inputDirection;
    public PlayerInputControl inputControl;
    public PlayerAnimation playerAnimation;
    
    public Rigidbody2D rb;
    public CapsuleCollider2D coll;
    public SpriteRenderer sprite;
    public float speed;
    public float jumpForce;
    public float walkSpeed;
    public PhysicsCheck physicsCheck;
    public float hurtForce;
    public float fadeDuration = 0.2f;    
    public float invisibleTime = 0.1f;
    private Coroutine fadeCoroutine;
    public float dashDistance;    // 冲刺距离
    public float dashDuration; // 冲刺持续时间
    public AudioClip jumpclip;


    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;


    [Header("状态")]
    public bool isLoading;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    
    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        rb = GetComponent<Rigidbody2D>();
        inputControl = new PlayerInputControl();
        playerAnimation=GetComponent<PlayerAnimation>();
        sprite = GetComponent<SpriteRenderer>();
        inputControl.GamePlay.Jump.started += Jump;
        inputControl.GamePlay.Slash.started += Slash;
        inputControl.GamePlay.Attack.started += PlayerAttack;

    }
    void Start()
    {
        
    }
    private void OnEnable()
    {
        inputControl.Enable();
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        loadDataEvent.OnEventRaised += OnLoadEvent;
    }

    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        loadDataEvent.OnEventRaised += OnLoadEvent;
    }

    private void OnLoadEvent()
    {
        inputControl.GamePlay.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
        CheckState();
    }
    public void exitStatus()
    {
        isHurt = false;
        isAttack = false;
        isDead = false;
        
    }
    private void FixedUpdate()
    {
        
        
        if (!isDead)
        {
            
            if (!isHurt && !isAttack)
            {
                Move();
            }
            if (isLoading)
            {
                inputControl.GamePlay.Disable();
            }
            else
            {
                inputControl.GamePlay.Enable();
            }
        }else
        {
            inputControl.GamePlay.Disable();
        }
    }
    
    public void Move()
    {
        float walkValue = inputControl.GamePlay.Walk.ReadValue<float>();
        
        float currentSpeed = (walkValue > 0.5f) ? walkSpeed : speed;
        
        
        rb.velocity = new Vector2(inputDirection.x * currentSpeed * Time.deltaTime,rb.velocity.y);
        
        int facedir = (int)transform.localScale.x;
        if(inputDirection.x < 0)
        {
            facedir = 1;
        }
        else if(inputDirection.x > 0) 
        {
            facedir = -1;
        }
        transform.localScale = new Vector3(facedir, 1, 1);
    }

    private void Jump(InputAction.CallbackContext context)
    {
       
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>()?.SetClip(jumpclip);
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }
    
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        playerAnimation.PlayAttack();
        isAttack = true;
    }
    private void Slash(InputAction.CallbackContext context)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutAndBack());
    }
    private IEnumerator FadeOutAndBack()
    {
        // 渐变到透明
        yield return StartCoroutine(FadeTo(0f, fadeDuration));
        // 停留一段时间
        yield return StartCoroutine(Dash(dashDistance, dashDuration));
        yield return new WaitForSeconds(invisibleTime);
        bool prevIsAttack = isAttack;
        isAttack = true;
        isAttack = prevIsAttack;
        yield return StartCoroutine(FadeTo(1f, fadeDuration));
        fadeCoroutine = null;
    }
    private IEnumerator Dash(float distance, float duration)
    {
        if (rb == null || duration <= 0f || Mathf.Approximately(distance, 0f))
            yield break;
        // 计算朝向：依据 transform.localScale.x 的符号推断朝向
        // 这里使用 -Mathf.Sign(localScale.x) 以配合你代码里 localScale 的方向约定（若不一致可改为 Mathf.Sign）
        float faceSign = Mathf.Sign(transform.localScale.x);
        Vector2 dir = new Vector2(-faceSign, 0f).normalized; // 朝向向量，向右为 (1,0)
        Vector2 start = rb.position;
        Vector2 target = start + dir * distance;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        // 保证最终位置
        rb.MovePosition(target);
    }
    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        Color c = sprite.color;
        float startAlpha = c.a;
        if (Mathf.Approximately(startAlpha, targetAlpha) || duration <= 0f)
        {
            c.a = targetAlpha;
            sprite.color = c;
            yield break;
        }
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            c.a = a;
            sprite.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        sprite.color = c;
    }
    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity=Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x),(transform.position.y-attacker.position.y)).normalized;
        rb.AddForce (dir*hurtForce, ForceMode2D.Impulse); 
        
    }
    public void PlayerDead()
    {
        isDead = true;
        inputControl.GamePlay.Disable();
        
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround?normal:wall;
    }
}
