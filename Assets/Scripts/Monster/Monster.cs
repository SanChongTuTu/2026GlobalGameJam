using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public CreateMonster monsterdata;

    // 状态枚举
    public enum State { Idle, Patrol, Chase, Attack }
    public State currentState = State.Idle;

    [Header("当前生命")]
    public float health = 100f;
    [Header("当前攻击")]
    public float damage = 10f;
    [Header("当前移动速度")]
    public float speed = 10f;

    [Header("检测是否受到伤害范围")]
    public List<Collider2D> monsterattackrange;
    [Header("地形检测")]
    public float frontRayDist = 0.8f;   // 前方墙检测距离
    public float groundRayDist = 1f;   // 脚下悬崖检测距离
    public LayerMask groundLayer;        // 地面层（含墙壁）

    protected Transform player;
    protected float patrolTimer;     
    protected Rigidbody2D rb;
    protected Animator anim;
    protected float idleTimer;
    protected float attackTimer;
    protected bool facingRight = true;
    protected bool attackFrame = false;
    protected float baseScaleX;
    protected Vector3 startPos;
    protected bool _isDead = false;

    public void Reset()//初始化怪物数据
    {
        player=GameDataManager.Instance.player;
        foreach (Collider2D collider in monsterattackrange)
        {
            collider.enabled = false;
        }

        health = monsterdata.health;
        damage = monsterdata.damage;
        speed = monsterdata.speed;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = transform.localScale.x >= 0;
        startPos = transform.position;
        player = GameObject.FindWithTag("Player")?.transform;

        facingRight = transform.localScale.x >= 0;
        baseScaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(facingRight ? baseScaleX : -baseScaleX,
                                            transform.localScale.y,
                                            transform.localScale.z);
    }

    public void LoadState()//随时读取状态
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        UpdateAnimator(distToPlayer);

        switch (currentState)
        {
            case State.Idle:
                IdleState(distToPlayer);
                break;
            case State.Patrol:
                PatrolState(distToPlayer);
                break;
            case State.Chase:
                ChaseState(distToPlayer);
                break;
            case State.Attack:
                AttackState(distToPlayer);
                break;
        }
    }

    public abstract void Attack(Collider2D other,int id);//这个抽象方法是触发器触发后调用的

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    public abstract void Die();

    #region 状态机功能实现抽象方法
    protected abstract void IdleState(float dist);//待机方法

    protected abstract void PatrolState(float dist);//巡逻方法

    protected abstract void ChaseState(float dist);//索敌方法

    protected abstract void AttackState(float dist);//攻击方法

    protected bool ShouldTurn(float moveDir)
    {
        // 1. 前方墙检测（水平射线）
        Vector2 origin = transform.position;
        Vector2 direction = new Vector2(moveDir, 0);
        RaycastHit2D hitWall = Physics2D.Raycast(origin, direction, frontRayDist, groundLayer);

        // 2. 脚下悬崖检测（斜向下射线）
        Vector2 groundOrigin = transform.position + Vector3.down * 0.1f + Vector3.right * (facingRight ? 0.5f : -0.5f);
        RaycastHit2D hitGround = Physics2D.Raycast(groundOrigin, Vector2.down, groundRayDist, groundLayer);

        // 有墙 或 脚下无地面 → 应该掉头
        return hitWall.collider != null || hitGround.collider == null;
    }

    public void Starttrigger(int id)
    {
        monsterattackrange[id].enabled = true;
    }

    public void Finishtrigger(int id)
    {
        monsterattackrange[id].enabled = false;
    }

    protected void FaceTo(float dirX)
    {
        if (dirX > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, 1);
        }
        else if (dirX < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, 1);
        }
    }

    protected void UpdateAnimator(float dist)
    {
        //anim.SetFloat("moveX", Mathf.Abs(rb.velocity.x));
        //anim.SetFloat("distanceToPlayer", dist);
        //anim.SetBool("canAttack", dist <= attackRange);
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        // 可视化范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, monsterdata.detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterdata.attackRange);

        // 前方墙检测
        Gizmos.color = Color.blue;
        Vector2 origin = transform.position;
        Vector2 dir = new Vector2(facingRight ? 1 : -1, 0);
        Gizmos.DrawRay(origin, dir * frontRayDist);

        // 脚下悬崖检测
        Gizmos.color = Color.cyan;
        Vector2 groundOrigin = transform.position + Vector3.down * 0.1f + Vector3.right * (facingRight ? 0.5f : -0.5f);
        Gizmos.DrawRay(groundOrigin, Vector2.down * groundRayDist);
    }
}
