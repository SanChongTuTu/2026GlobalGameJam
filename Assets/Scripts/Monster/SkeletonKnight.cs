using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class SkeletonKnight : Monster
{
    [Header("骷髅骑士特有属性")]
    [SerializeField] private float berserkThreshold = 0.5f;
    [SerializeField] private bool isBerserk = false;
    private bool hasTriggeredBerserk = false;
    private float berserkSpeedMultiplier = 1.5f;

    [Header("帧伤技能")]
    [SerializeField] private bool isFrameDamageActive = false;
    [SerializeField] private float frameDamageDuration = 3f;
    [SerializeField] private float frameDamageCooldown = 6f;
    [SerializeField] private float frameDamageInterval = 0.3f;
    [SerializeField] private int frameDamageValue = 5;
    [SerializeField] private GameObject frameDamageEffect;

    private float lastFrameDamageTime;
    private float frameDamageEndTime;
    private float lastFrameDamageSkillTime;
    private Collider2D currentTarget;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("眩晕技能")]
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private float stunCooldown = 5f;
    [SerializeField] private float stunRange = 1.2f;
    private float lastStunTime;
    private bool isStunning = false;

    [Header("状态")]
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        Reset();
        lastStunTime = -stunCooldown;
        lastFrameDamageSkillTime = -frameDamageCooldown;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (!isDead)
        {
            LoadState();
            UpdateFrameDamage();
            UpdateFrameDamageSkill();

            // 检查狂暴状态（基于血量）
            if (!hasTriggeredBerserk && health <= monsterdata.health * berserkThreshold)
            {
                EnterBerserkMode();
            }
        }
    }

    private void UpdateFrameDamageSkill()
    {
        if (isFrameDamageActive && Time.time >= frameDamageEndTime)
        {
            DisableFrameDamage();
        }
    }

    private void ActivateFrameDamageSkill()
    {
        if (Time.time - lastFrameDamageSkillTime < frameDamageCooldown) return;
        if (isStunning || isAttacking) return;

        lastFrameDamageSkillTime = Time.time;
        isFrameDamageActive = true;
        frameDamageEndTime = Time.time + frameDamageDuration;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
        if (frameDamageEffect != null)
        {
            frameDamageEffect.SetActive(true);
        }

        // 可选：播放一个简单特效，不需要单独动画
        // anim.SetTrigger("CastFrameDamage");
    }

    private void DisableFrameDamage()
    {
        isFrameDamageActive = false;
        currentTarget = null;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        if (frameDamageEffect != null)
        {
            frameDamageEffect.SetActive(false);
        }
    }

    private void UpdateFrameDamage()
    {
        if (!isFrameDamageActive || currentTarget == null) return;

        if (!currentTarget.IsTouching(GetComponent<Collider2D>()))
        {
            currentTarget = null;
            return;
        }

        if (Time.time - lastFrameDamageTime >= frameDamageInterval)
        {
            lastFrameDamageTime = Time.time;
            BasicControl player = currentTarget.GetComponent<BasicControl>();
            if (player != null)
            {
                player.TakeDamage(frameDamageValue);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isFrameDamageActive && collision.gameObject.CompareTag("Player"))
        {
            if (currentTarget == null)
            {
                currentTarget = collision.collider;
                lastFrameDamageTime = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentTarget == collision.collider)
        {
            currentTarget = null;
        }
    }

    private IEnumerator StunAttack()
    {
        if (isStunning || isAttacking) yield break;

        isStunning = true;
        lastStunTime = Time.time;

        // 不需要眩晕动画，只用代码实现眩晕效果
        // 让骷髅骑士停顿一下作为视觉反馈
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.3f);

        if (player != null && Vector2.Distance(transform.position, player.position) <= stunRange)
        {
            BasicControl pc = player.GetComponent<BasicControl>();
            if (pc != null)
            {
                pc.Stun(stunDuration);
            }
        }

        yield return new WaitForSeconds(0.5f);
        isStunning = false;
    }

    private void EnterBerserkMode()
    {
        if (hasTriggeredBerserk) return;

        isBerserk = true;
        hasTriggeredBerserk = true;
        speed *= berserkSpeedMultiplier;

        frameDamageCooldown = 4f;
        frameDamageValue = 8;

        // 狂暴时闪红光（不需要单独动画）
        if (spriteRenderer != null)
        {
            StartCoroutine(BerserkFlash());
        }
    }

    private IEnumerator BerserkFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void Attack(Collider2D other, int id)
    {
        if (isDead) return;

        BasicControl player = other.GetComponent<BasicControl>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;
        _isDead = true;

        DisableFrameDamage();

        anim.SetBool("Move", false);
        anim.SetTrigger("Die");

        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

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
        if (isDead) return;
        anim.SetBool("Move", false);
        if (!openblood)
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
        if (isDead) return;

        if (dist < monsterdata.detectRange)
        {
            currentState = State.Chase;
            anim.SetBool("Move", true);
            return;
        }

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
        if (isDead || isStunning) return;

        blood.SetActive(true);

        // 帧伤技能决策
        if (!isFrameDamageActive &&
            Time.time - lastFrameDamageSkillTime >= frameDamageCooldown * 0.7f &&
            Random.value < 0.003f)
        {
            ActivateFrameDamageSkill();
        }

        // 眩晕技能决策
        if (!isStunning && Time.time - lastStunTime >= stunCooldown && Random.value < 0.002f)
        {
            StartCoroutine(StunAttack());
            return;
        }

        if (dist <= monsterdata.attackRange)
        {
            float moveDirS = player.position.x > transform.position.x ? 1 : -1;
            if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDirS *= 0.5f;
            rb.velocity = new Vector2(moveDirS * speed * 0.8f, rb.velocity.y);
            anim.SetBool("Move", false);
            currentState = State.Attack;
            return;
        }
        if (dist > monsterdata.detectRange * 1.5f && !openblood)
        {
            anim.SetBool("Move", false);
            currentState = State.Idle;
            return;
        }

        float dirX = player.position.x > transform.position.x ? 1 : -1;

        Vector2 origin = transform.position + Vector3.up * 0.15f;
        Vector2 ledgeDir = new Vector2(dirX, -1).normalized;
        bool groundHere = Physics2D.Raycast(transform.position, Vector2.down, downCheckDist, groundLayer);
        bool wallAhead = Physics2D.Raycast(origin, new Vector2(dirX, 0), wallCheckDist, groundLayer);

        Vector2 ledgeOrigin = (Vector2)transform.position + Vector2.up * 0.15f;
        ContactFilter2D ledgeFilter = new ContactFilter2D();
        ledgeFilter.useNormalAngle = true;
        ledgeFilter.minNormalAngle = -30;
        ledgeFilter.maxNormalAngle = 30;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int c = Physics2D.Raycast(ledgeOrigin, ledgeDir, ledgeFilter, hits, ledgeCheckDist);
        bool ledgeEmpty = c == 0;

        bool needJump = (ledgeEmpty && groundHere) || wallAhead;
        if (needJump && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            float jumpForce = isBerserk ? jumpVelocity * 1.2f : jumpVelocity;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            return;
        }

        float moveDir = player.position.x > transform.position.x ? 1 : -1;
        if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDir *= 0.5f;
        rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        FaceTo(dirX);
    }

    protected override void AttackState(float dist)
    {
        if (isDead) return;

        blood.SetActive(true);
        attackTimer += Time.deltaTime;

        if (attackTimer >= monsterdata.attackCooldown)
        {
            attackTimer = 0;
            isAttacking = true;

            float dirX = player.position.x > transform.position.x ? 1 : -1;
            float moveDir = player.position.x > transform.position.x ? 1 : -1;
            if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDir *= 0.5f;
            rb.velocity = new Vector2(moveDir * speed * 0.5f, rb.velocity.y);
            FaceTo(dirX);

            // 根据狂暴状态选择攻击动画
            if (isBerserk)
            {
                anim.SetTrigger("AttackTwo");
            }
            else
            {
                anim.SetTrigger("AttackOne");
            }

            StartCoroutine(ResetAttackFlag());

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

        if (player != null)
        {
            float dirX = player.position.x > transform.position.x ? 1 : -1;
            FaceTo(dirX);
        }
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    #endregion
}