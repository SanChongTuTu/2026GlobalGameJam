using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class SkeletonKnight : Monster
{
    [Header("骷髅骑士特有属性")]
    [SerializeField] private float berserkThreshold = 0.5f; // 狂暴阈值(50%血量)
    private bool hasTriggeredBerserk = false; // 是否已触发狂暴（永久性）
    private float berserkSpeedMultiplier = 1.5f; // 狂暴速度倍率

    void Start()
    {
        Reset();
    }

    void Update() => LoadState();

    // 处理受伤 - 匹配基类签名
    public new void TakeDamage(float amount, float? hitstuntime = 0, Vector2? knockBackDir = null)
    {
        base.TakeDamage(amount, hitstuntime, knockBackDir);

        // 检查是否需要进入狂暴状态
        if (!hasTriggeredBerserk && health <= monsterdata.health * berserkThreshold)
        {
            EnterBerserkMode();
        }
    }

    // 进入狂暴状态 - 永久性，只提升速度，动画通过 hasTriggeredBerserk 区分
    private void EnterBerserkMode()
    {
        if (hasTriggeredBerserk) return;

        hasTriggeredBerserk = true;
        speed *= berserkSpeedMultiplier;

        Debug.Log("骷髅骑士进入狂暴状态！攻击和速度提升！");
    }

    public override void Attack(Collider2D other, int id)
    {
        other.GetComponent<BasicControl>()?.TakeDamage(damage);
    }

    public override void Die()
    {
        _isDead = true;

        // 停止移动
        rb.velocity = Vector2.zero;
        anim.SetBool("Move", false);
        anim.SetTrigger("Die");

        // 禁用碰撞和AI
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    #region 行为状态实现

    protected override void IdleState(float dist)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("Move", false);

        // 发现玩家，进入追击
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
        // 发现玩家，立即追击
        if (dist < monsterdata.detectRange)
        {
            currentState = State.Chase;
            return;
        }

        patrolTimer += Time.deltaTime;
        float moveDir = facingRight ? 1 : -1;

        // 减速效果
        if (effects.Exists(e => e.effectname == "Slow"))
            moveDir *= 0.5f;

        rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);

        // 遇到障碍或悬崖，回头进入Idle
        if (ShouldTurn(moveDir))
        {
            patrolTimer = 0;
            FaceTo(-moveDir);
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }

        // 巡逻时间结束，回头Idle
        if (patrolTimer >= monsterdata.patrolDuration)
        {
            patrolTimer = 0;
            FaceTo(-moveDir);
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }
    }

    protected override void ChaseState(float dist)
    {
        // 进入攻击范围，切换攻击状态（Move动画保持，AttackState会处理）
        if (dist <= monsterdata.attackRange)
        {
            currentState = State.Attack;
            attackTimer = monsterdata.attackCooldown; // 进入立即攻击
            return;
        }

        // 丢失目标，回到Idle
        if (dist > monsterdata.detectRange * 1.5f)
        {
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }

        // 计算方向
        float dirX = player.position.x > transform.position.x ? 1 : -1;
        float moveDir = dirX;

        // 减速效果
        if (effects.Exists(e => e.effectname == "Slow"))
            moveDir *= 0.5f;

        // 环境检测（悬崖和墙壁）
        Vector2 origin = transform.position + Vector3.up * 0.15f;
        Vector2 ledgeDir = new Vector2(dirX, -1).normalized;
        bool groundHere = Physics2D.Raycast(transform.position, Vector2.down, downCheckDist, groundLayer);
        bool wallAhead = Physics2D.Raycast(origin, new Vector2(dirX, 0), wallCheckDist, groundLayer);

        ContactFilter2D ledgeFilter = new ContactFilter2D();
        ledgeFilter.useNormalAngle = true;
        ledgeFilter.minNormalAngle = -30;
        ledgeFilter.maxNormalAngle = 30;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int c = Physics2D.Raycast(origin, ledgeDir, ledgeFilter, hits, ledgeCheckDist);
        bool ledgeEmpty = c == 0;

        // 需要跳跃（遇到障碍或悬崖）
        if ((ledgeEmpty && groundHere) || wallAhead)
        {
            if (Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            }
        }

        // 持续追击移动
        rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        FaceTo(dirX);

        // 确保Move动画播放
        anim.SetBool("Move", true);
    }

    protected override void AttackState(float dist)
    {
        // 玩家逃出攻击范围
        if (dist > monsterdata.attackRange)
        {
            // 如果还在检测范围内，追击；否则Idle
            if (dist < monsterdata.detectRange)
            {
                anim.SetBool("Move", true);
                currentState = State.Chase;
            }
            else
            {
                anim.SetBool("Move", false);
                currentState = State.Idle;
            }
            return;
        }

        // 攻击冷却计时
        attackTimer += Time.deltaTime;

        // 冷却结束，执行攻击
        if (attackTimer >= monsterdata.attackCooldown)
        {
            attackTimer = 0;

            // 面向玩家
            float dirX = player.position.x > transform.position.x ? 1 : -1;
            FaceTo(dirX);

            // 根据狂暴状态选择攻击动画（关键逻辑）
            if (hasTriggeredBerserk)
            {
                anim.SetTrigger("AttackTwo");
                Debug.Log("狂暴攻击！");
            }
            else
            {
                anim.SetTrigger("AttackOne");
            }
        }

        player.GetComponent<BasicControl>()?.TakeDamage(damage);

        
        if (dist > monsterdata.attackRange * 0.5f)
        {
            float approachDir = player.position.x > transform.position.x ? 1 : -1;
            if (effects.Exists(e => e.effectname == "Slow"))
                approachDir *= 0.5f;
            rb.velocity = new Vector2(approachDir * speed * 0.3f, rb.velocity.y);
        }
        else
        {
            // 距离够近，停止移动专注攻击
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // 攻击期间保持Move为false（播放攻击动画）
        anim.SetBool("Move", false);
    }

    #endregion
}