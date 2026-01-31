using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Monster : MonoBehaviour
{
    public CreateMonster monsterdata;

    // ״̬ö��
    public enum State { Idle, Patrol, Chase, Attack }
    public State currentState = State.Idle;

    [System.Serializable]
    public class Effect
    {
        public string effectname;
        public float time;

        public void DecreaseTime(float deltaTime)
        {
            time -= deltaTime;
        }
    }

    [Header("��ǰ����")]
    public float health = 100f;
    [Header("��ǰ����")]
    public float damage = 10f;
    [Header("��ǰ�ƶ��ٶ�")]
    public float speed = 10f;
    [Header("Sprite")]
    public SpriteRenderer render;
    [Header("bloodbar")]
    public GameObject blood;
    [SerializeField] private float fadetime;
    [Header("��ǰbuff/debuff")]
    public List<Effect> effects = new List<Effect>();

    [Header("����Ƿ��ܵ��˺���Χ")]
    public List<Collider2D> monsterattackrange;

    public LayerMask groundLayer;        // ����㣨��ǽ�ڣ�

    protected bool openblood = false;
    protected Transform player;
    protected float patrolTimer;     
    protected Rigidbody2D rb;
    protected Animator anim;
    protected float idleTimer;
    protected float attackTimer;
    protected bool facingRight = true;
    protected bool attackFrame = false;
    protected float baseScaleX;
    protected float baseScaleblood;
    protected Vector3 startPos;
    protected bool _isDead = false;

    [Header("跳跃")]
    public float jumpVelocity = 12f;   // 起跳速度
    public float wallCheckDist = 1f; // 前方探墙距离
    public float ledgeCheckDist = 1f;// 斜下探空缺距离
    public float downCheckDist = 1f;// 下方空缺距离
    [Header("射线起点偏移")]
    public Vector2 rayStartOffset = new Vector2(0, 0.15f); // x=左右，y=上下
    [Header("击飞")]
    public bool canBeKnocked = true;    // 是否可以被击飞
    [Header("是否会被控制")]
    public bool canbeHitStun = true;    // 是否可以硬直

    protected float hitStunTimer = 0f;  // 剩余硬直

    private Image bloodback;
    private Image bloodquick;
    private Image bloodslow;

    private float maxsizex;

    public void Reset()//��ʼ����������
    {
        player=GameDataManager.Instance.player;
        foreach (Collider2D collider in monsterattackrange)
        {
            collider.enabled = false;
        }
        bloodback=blood.transform.GetChild(0).GetComponent<Image>();
        bloodslow=blood.transform.GetChild(1).GetComponent<Image>();
        bloodquick=blood.transform.GetChild(2).GetComponent<Image>();
        maxsizex = bloodback.rectTransform.sizeDelta.x;
        bloodquick.rectTransform.sizeDelta = new Vector2(maxsizex, bloodquick.rectTransform.sizeDelta.y);
        bloodslow.rectTransform.sizeDelta = new Vector2(maxsizex, bloodslow.rectTransform.sizeDelta.y);
        blood.gameObject.SetActive(false);

        canbeHitStun = monsterdata.canbeHitStun;
        canBeKnocked = monsterdata.canBeKnocked;
        health = monsterdata.health;
        damage = monsterdata.damage;
        speed = UnityEngine.Random.Range(monsterdata.speed.x,monsterdata.speed.y+1);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = transform.localScale.x >= 0;
        startPos = transform.position;
        player = GameObject.FindWithTag("Player")?.transform;

        facingRight = transform.localScale.x >= 0;
        baseScaleX = Mathf.Abs(transform.localScale.x);
        baseScaleblood = Mathf.Abs(blood.transform.localScale.x);
        transform.localScale = new Vector3(facingRight ? baseScaleX : -baseScaleX,
                                            transform.localScale.y,
                                            transform.localScale.z);
    }

    public void LoadState()//��ʱ��ȡ״̬
    {
        if (player == null) return;
        UpdateBloodBar();
        if (_isDead)
        {
            anim.speed = 1;
            return;
        }
        var toRemove = new List<Effect>();
        foreach (var effect in effects)
        {
            switch (effect.effectname)
            {

            }
            effect.DecreaseTime(Time.deltaTime);
            if (effect.time <= 0)
            {
                toRemove.Add(effect);
            }
        }
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            switch (toRemove[i].effectname)
            {

            }

            effects.Remove(toRemove[i]);
        }


        if (effects.Contains(effects.Find(e => e.effectname == "Slow"||e.effectname=="Freeze"))&&canbeHitStun)
        {
            render.color = Color.blue;
        }
        else
        {
            render.color = Color.white;
        }

        if(effects.Contains(effects.Find(e => e.effectname == "Freeze")) && canbeHitStun)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            hitStunTimer=effects.Find(e => e.effectname == "Freeze").time;
        }

        if (hitStunTimer>0&&canbeHitStun)
        {
            if(_isDead) return;
            hitStunTimer -= Time.deltaTime;
            anim.speed = 0f;
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(info.shortNameHash, 0, info.normalizedTime);   
            if (hitStunTimer <= 0)
            {
                hitStunTimer = 0;
                anim.speed = 1;
            }
            return;  
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

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

    public abstract void Attack(Collider2D other,int id);//������󷽷��Ǵ�������������õ�

    public void TakeDamage(float amount, float? hitstuntime = 0, Vector2? knockBackDir = null)
    {
        if (_isDead) return;

        blood.SetActive(true);
        openblood = true;
        health -= amount;
        if (canbeHitStun)
        {
            hitStunTimer = (float)hitstuntime;
        }
        // 真正给一次击退速度
        // 击飞逻辑
        if (knockBackDir.HasValue&&canBeKnocked)
        {
            rb.velocity = knockBackDir.Value;
        }

        if (health <= 0) Die();
    }

    public void SetEffect(string name,float time)
    {
        if(effects.Exists(e => e.effectname == name))
        {
            var existingEffect = effects.Find(e => e.effectname == name);
            existingEffect.time = time;
            return;
        }
        effects.Add(new Effect { effectname = name, time = time });
    }

    public abstract void Die();

    public void Destroythis()
    {
        Destroy(gameObject);
    }

    #region ״̬������ʵ�ֳ��󷽷�
    protected abstract void IdleState(float dist);//��������

    protected abstract void PatrolState(float dist);//Ѳ�߷���

    protected abstract void ChaseState(float dist);//���з���

    protected abstract void AttackState(float dist);//��������

    protected bool ShouldTurn(float moveDir)
    {
        Vector2 origin = (Vector2)transform.position + rayStartOffset;
        /* 1. 前方水平探墙 */
        RaycastHit2D hitWall = Physics2D.Raycast(origin,
                                                 new Vector2(moveDir, 0),
                                                 wallCheckDist,
                                                 groundLayer);

        /* 2. 斜下方探空（45°）→ 前方是悬崖 */
        Vector2 ledgeDir = new Vector2(moveDir, -1).normalized;
        bool ledgeEmpty = !Physics2D.Raycast(origin, ledgeDir, ledgeCheckDist, groundLayer);


        // 有墙 或 前方是悬崖 或 脚下没地 → 应该回头
        return hitWall.collider != null || ledgeEmpty;
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
    #endregion

    protected void UpdateBloodBar()
    {
        blood.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x)*baseScaleblood, blood.transform.localScale.y, blood.transform.localScale.z);
        bloodquick.rectTransform.sizeDelta = new Vector2(health / monsterdata.health * maxsizex, bloodquick.rectTransform.sizeDelta.y);
        //StartCoroutine(Updatebloodbar());
    }

    //protected IEnumerator Updatebloodbar()
    //{
    //    float nowsizex = bloodslow.rectTransform.sizeDelta.x;
    //    float time = 0;
    //    while (time < fadetime)
    //    {
    //        time += Time.deltaTime;
    //        float percent = time / fadetime;
    //        float sizex = Mathf.Lerp(nowsizex, bloodquick.rectTransform.sizeDelta.x, percent);
    //        bloodslow.rectTransform.sizeDelta = new Vector2(sizex, bloodslow.rectTransform.sizeDelta.y);
    //        yield return null;
    //    }
    //    bloodslow.rectTransform.sizeDelta = new Vector2(health / monsterdata.health * maxsizex, bloodslow.rectTransform.sizeDelta.y);
    //    yield break;
    //}

    protected virtual void OnDrawGizmosSelected()
    {
        // ���ӻ���Χ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, monsterdata.detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterdata.attackRange);

        //// ǰ��ǽ���
        //Gizmos.color = Color.blue;
        //Vector2 origin = transform.position;
        //Vector2 dir = new Vector2(facingRight ? 1 : -1, 0);
        //Gizmos.DrawRay(origin, dir * frontRayDist);

        //// �������¼��
        //Gizmos.color = Color.cyan;
        //Vector2 groundOrigin = transform.position + Vector3.down * 0.1f + Vector3.right * (facingRight ? 0.5f : -0.5f);
        //Gizmos.DrawRay(groundOrigin, Vector2.down * groundRayDist);

        // 1. 起点（白色小圆）
        Vector2 origin = (Vector2)transform.position + rayStartOffset;
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(origin, 0.05f); // 一个点

        // 2. 前方水平探墙
        Gizmos.color = Color.blue;
        Vector2 frontDir = new Vector2(facingRight ? 1 : -1, 0);
        Gizmos.DrawRay(origin, frontDir * wallCheckDist);

        // 3. 斜下探空（洋红色）
        Gizmos.color = Color.magenta;
        Vector2 ledgeDir = new Vector2(facingRight ? 1 : -1, -1).normalized;
        Gizmos.DrawRay(origin, ledgeDir * ledgeCheckDist);

        // 4. 正下方探地（绿色）
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector2.down * downCheckDist);
    }
}
