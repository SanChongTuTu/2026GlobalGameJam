using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class WisdomMask : Monster
{
    public List<GameObject> bullet;
    public float bulletSpeed = 5f;
    public bool startstrength = false;
    public int[] attackdesformer = new int[6]{ 30, 50, 0,20,0,0 };//觉醒前5种攻击概率
    public int[] attackdeslate = new int[6] { 0,0,40, 0, 20,40 };

    bool startattacktimeadd = true;
    float attackcooldown = 5;

    void Start(){
        Reset();
        startattacktimeadd = true;
        attackcooldown = monsterdata.attackCooldown;
    }

    void Update()
    {
        LoadState();
        if(health <= monsterdata.health / 2&&!startstrength)
        {
            startstrength = true;
            attackcooldown /= 2;
            anim.SetTrigger("Strength");
        }
    }

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
            rb.velocity = Vector2.zero;
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
        float rawDeltaX = player.position.x - transform.position.x;
        FaceTo(rawDeltaX);
    }

    protected override void AttackState(float dist)
    {
        blood.SetActive(true);

        if(startattacktimeadd)
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackcooldown)
        {
            startattacktimeadd = false;
            attackTimer = 0;
            float dirX = player.position.x > transform.position.x ? 1 : -1;
            float moveDir = player.position.x > transform.position.x ? 1 : -1;
            if (effects.Contains(effects.Find(e => e.effectname == "Slow"))) moveDir *= 0.5f;
            rb.velocity = new Vector2(moveDir * speed * 0.1f, rb.velocity.y);
            FaceTo(dirX);

            int[] weight = startstrength ? attackdeslate : attackdesformer;
            int rand = Random.Range(0, 100);          // 0~99
            int cum = 0;

            if (rand < weight[0])                      // Attack1
                anim.SetTrigger("Attack");
            else if ((cum += weight[0]) >= 0 && rand < cum + weight[1])  // Attack2
                anim.SetTrigger("Attack2");
            else if ((cum += weight[1]) >= 0 && rand < cum + weight[2])  // Attack3
                anim.SetTrigger("Attack3");
            else if ((cum += weight[2]) >= 0 && rand < cum + weight[3])  // Attack4
                anim.SetTrigger("Attack4");
            else if ((cum += weight[3]) >= 0 && rand < cum + weight[4])  // Attack5
                anim.SetTrigger("Attack5");
            else                                                        // Attack6
                anim.SetTrigger("Attack6");

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

    public void StartAttacktimeadd()
    {
        startattacktimeadd = true;
    }

    public void ShootHigh(int id)
    {
        GameObject fb = Instantiate(bullet[id], transform.position+new Vector3(0,2,0), transform.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = transform.right * bulletSpeed * Mathf.Sign(transform.localScale.x);
    }

    public void ShootMid(int id)
    {
        GameObject fb = Instantiate(bullet[id], transform.position, transform.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = transform.right * bulletSpeed * Mathf.Sign(transform.localScale.x);
    }

    public void ShootLow(int id)
    {
        GameObject fb = Instantiate(bullet[id], transform.position + new Vector3(0, -2, 0), transform.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = transform.right * bulletSpeed * Mathf.Sign(transform.localScale.x);
    }

    public void ShootBullet(float offset)
    {
        GameObject fb = Instantiate(bullet[0], transform.position + new Vector3(0, offset, 0), transform.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = transform.right * bulletSpeed * Mathf.Sign(transform.localScale.x);
    }

    public void ShootBulletBack(float offset)
    {
        GameObject fb = Instantiate(bullet[0], transform.position + new Vector3(0, offset, 0), transform.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = transform.right * bulletSpeed * -Mathf.Sign(transform.localScale.x);
    }

    public void SummonRandom()
    {
        int id = Random.Range(1,4);
        GameObject fb = Instantiate(bullet[id], transform.position, transform.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = transform.right * bulletSpeed * Mathf.Sign(transform.localScale.x);
    }

    public void ShootSky()
    {
        int rand = Random.Range(6, 11);                
        Vector3 playerPos = GameDataManager.Instance.player.position; 

        for (int i = 0; i < rand; i++)
        {
            // 1. 基础方向（发射点→玩家）
            Vector3 dirToPlayer = (playerPos - (transform.position + Vector3.up * 4)).normalized;

            // 2. 在该方向上做 ±30° 随机散射
            float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            float scatter = Random.Range(-30f, 30f);
            float finalAngle = baseAngle + scatter;

            Quaternion rot = Quaternion.AngleAxis(finalAngle, Vector3.forward);

            // 3. 生成并给速度
            GameObject fb = Instantiate(bullet[0], transform.position + Vector3.up * 5, rot);
            Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
            rb.velocity = fb.transform.right * bulletSpeed*1.2f;   // 子弹右方向即 finalAngle
        }
    }
}