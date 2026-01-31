using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Snake : Monster
{

    void Start()=>Reset();

    void Update() => LoadState();

    public override void Attack(Collider2D other, int id)
    {
        other.GetComponent<BasicControl>()?.TakeDamage(damage);
    }

    public override void Die()
    {
        _isDead = true;
        anim.SetTrigger("Die");
    }

    #region 行为状态实现
    protected override void IdleState(float dist)
    {
        if(!openblood)
        blood.SetActive(false);
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (dist < monsterdata.detectRange)
        {
            anim.SetBool("Move", true);
            currentState = State.Chase;
            return;
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= monsterdata.idleTime)
        {
            idleTimer = 0;
            anim.SetBool("Move", true);
            currentState = State.Patrol;
            facingRight = Random.value > 0.5f;
            float moveDir = facingRight ? 1 : -1;
            transform.localScale = new Vector3(moveDir * baseScaleX, transform.localScale.y, 1);
        }
    }

    protected override void PatrolState(float dist)
    {
        if (dist < monsterdata.detectRange) { currentState = State.Chase; anim.SetBool("Move", true); return; }

        patrolTimer += Time.deltaTime;
        float moveDir = facingRight ? 1 : -1;
        if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDir *= 0.5f;
        rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        if (ShouldTurn(moveDir))
        {
            patrolTimer = 0;
            FaceTo(-moveDir);
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }
        if (patrolTimer >= monsterdata.patrolDuration)
        {
            patrolTimer = 0;
            FaceTo(-moveDir);
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }
        if (Random.value < 0.0001f)
        {
            currentState = State.Idle;
            anim.SetBool("Move", false);
        }
        }

    protected override void ChaseState(float dist)
    {
        blood.SetActive(true);
        if (dist <= monsterdata.attackRange)
        {
            float moveDirS = player.position.x > transform.position.x ? 1 : -1;
            if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDirS *= 0.5f;
            rb.velocity = new Vector2(moveDirS * speed * 0.8f, rb.velocity.y);
            anim.SetBool("Move", false);
            currentState = State.Attack;
            return;
        }
        if (dist > monsterdata.detectRange * 1.5f&&!openblood)
        {
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }
        float dirX = player.position.x > transform.position.x ? 1 : -1;

        Vector2 origin = transform.position + Vector3.up * 0.15f;

        Vector2 ledgeDir = new Vector2(dirX, -1).normalized;
        //bool ledgeEmpty = !Physics2D.Raycast(origin, ledgeDir, ledgeCheckDist, groundLayer);

        // 1. 脚下探地——用角色脚底，不要抬高
        bool groundHere = Physics2D.Raycast(transform.position, Vector2.down, downCheckDist, groundLayer);

        bool wallAhead = Physics2D.Raycast(origin, new Vector2(dirX, 0), wallCheckDist, groundLayer);

        Vector2 ledgeOrigin = (Vector2)transform.position + Vector2.up * 0.15f;

        // 新写法：过滤法线，墙根/垂直面直接忽略
        ContactFilter2D ledgeFilter = new ContactFilter2D();
        ledgeFilter.useNormalAngle = true;
        ledgeFilter.minNormalAngle = -30;   // 只收 -30°~30° 的法线（水平地面）
        ledgeFilter.maxNormalAngle = 30;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int c = Physics2D.Raycast(ledgeOrigin, ledgeDir, ledgeFilter, hits, ledgeCheckDist);
        bool ledgeEmpty = c == 0;            // 0 表示没扫到水平地面 → 前方是悬崖

        bool needJump = (ledgeEmpty && groundHere) || wallAhead; 
        if (needJump && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            return;   
        }

        //if (ShouldTurn(dirX))
        //{
        //    rb.velocity = Vector2.zero;
        //    FaceTo(-dirX);
        //    return;
        //}
        float moveDir = player.position.x > transform.position.x ? 1 : -1;
        if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDir *= 0.5f;
        rb.velocity = new Vector2(moveDir * speed * 1f, rb.velocity.y);
        FaceTo(dirX);
    }

    protected override void AttackState(float dist)
    {
        blood.SetActive(true);
        attackTimer += Time.deltaTime;

        if (attackTimer >= monsterdata.attackCooldown)
        {
            attackTimer = 0;
            float dirX = player.position.x > transform.position.x ? 1 : -1;
            float moveDir = player.position.x > transform.position.x ? 1 : -1;
            if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDir *= 0.5f;
            rb.velocity = new Vector2(moveDir * speed * 0.8f, rb.velocity.y);
            FaceTo(dirX);
            anim.SetTrigger("Attack");//播放攻击动画,打开触发器，如果触发执行上面的Attack函数
            if (dist <= monsterdata.attackRange)
            {
                anim.SetBool("Move", false);
                currentState = State.Attack;
            }
            else
            {
                anim.SetBool("Move", true);
                currentState = State.Chase;
            }
        }
    }
    #endregion
}