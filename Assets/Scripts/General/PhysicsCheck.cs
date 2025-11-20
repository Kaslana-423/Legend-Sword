using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    [Header("¼ì²â²ÎÊý")]
    public bool manual;
    public float checkRaduis;
    public LayerMask groundLayer;
    public Vector2 bottomOffset; 
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    [Header("×´Ì¬")]
    public bool isGround;
    public bool isWallLeft;
    public bool isWallRight;

    // Start is called before the first frame update
    void Awake() 
    {
        coll = GetComponent<CapsuleCollider2D>();
        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x ) / 2 + coll.offset.x, (coll.bounds.size.y ) / 2);
            leftOffset = new Vector2(-(coll.bounds.size.x) / 2 + coll.offset.x, rightOffset.y);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Check();
    } 
    public void Check()
    {
       isGround = Physics2D.OverlapCircle((Vector2)transform.position+bottomOffset,checkRaduis,groundLayer);
       isWallLeft = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
       isWallRight = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset,checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset,checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset,checkRaduis);

    }
}
