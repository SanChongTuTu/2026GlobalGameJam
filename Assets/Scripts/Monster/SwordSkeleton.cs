using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class SwordSkeleton : Monster
{

    void Start()
    {
        Reset();
    }

    void Update()
    {
        LoadState();
    }

    public override void Attack(Collider2D other, int id)
    {
        other.GetComponent<BasicControl>()?.TakeDamage(damage);
    }

    public override void Die()
    {
        _isDead = true;

        Destroy(gameObject);
    }

    #region 行为状态实现
    protected override void IdleState(float dist)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (dist < monsterdata.detectRange)
        {
            currentState = State.Chase;
            return;
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= monsterdata.idleTime)
        {
            idleTimer = 0;
            currentState = State.Patrol;
            facingRight = Random.value > 0.5f;
            float moveDir = facingRight ? 1 : -1;
            transform.localScale = new Vector3(moveDir * baseScaleX, transform.localScale.y, 1);
        }
    }

    protected override void PatrolState(float dist)
    {
        if (dist < monsterdata.detectRange) { currentState = State.Chase; return; }

        patrolTimer += Time.deltaTime;
        float moveDir = facingRight ? 1 : -1;
        rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        if (ShouldTurn(moveDir))
        {
            patrolTimer = 0;
            FaceTo(-moveDir);   
            currentState = State.Idle;
            return;
        }
        if (patrolTimer >= monsterdata.patrolDuration)
        {
            patrolTimer = 0;
            FaceTo(-moveDir);
            currentState = State.Idle;
            return;
        }
        if (Random.value < 0.0001f) currentState = State.Idle;
    }

    protected override void ChaseState(float dist)
    {
        if (dist <= monsterdata.attackRange)
        {
            rb.velocity = Vector2.zero;
            currentState = State.Attack;
            return;
        }
        if (dist > monsterdata.detectRange * 1.5f)
        {
            currentState = State.Idle;
            return;
        }
        float dirX = player.position.x > transform.position.x ? 1 : -1;
        if (ShouldTurn(dirX))
        {
            rb.velocity = Vector2.zero;
            FaceTo(-dirX);
            return;
        }

        rb.velocity = new Vector2(dirX * speed * 1.5f, rb.velocity.y);
        FaceTo(dirX);
    }

    protected override void AttackState(float dist)
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= monsterdata.attackCooldown)
        {
            attackTimer = 0;
            float dirX = player.position.x > transform.position.x ? 1 : -1;
            rb.velocity = new Vector2(dirX * speed * 1.5f, rb.velocity.y);
            FaceTo(dirX);
            anim.SetTrigger("Attack");//播放攻击动画,打开触发器，如果触发执行上面的Attack函数
            currentState = (dist <= monsterdata.attackRange) ? State.Attack : State.Chase;
        }
    }
    #endregion
}