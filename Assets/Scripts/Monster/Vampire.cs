using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Vampire : Monster
{
    [Header("Vampire Unique Settings")]
    [Header("Critical Hit Settings")]
    public float critChance = 0.2f;
    public float critMultiplier = 2.0f;
    public float megaCritMultiplier = 4.0f;
    private bool _nextHitIsMegaCrit = false;

    [Header("Flight Boundaries")]
    public Vector2 flightAreaSize = new Vector2(10f, 6f);
    private Vector2 _flightMin;
    private Vector2 _flightMax;
    private Vector3 _targetPatrolPoint;

    [Header("Leech & Speed Settings")]
    public float meleeLeechRate = 0.3f;
    public float speedLeechMultiplier = 1.5f;
    public float speedLeechDuration = 3f;
    private float _baseSpeed;

    [Header("Mist Form Settings")]
    public float mistFormDuration = 2f;
    public float mistFormRepositionHeight = 3f;
    private bool _mistFormTriggered = false;
    private bool _isInvulnerable = false;

    private SpriteRenderer _sr;
    private bool _isReturning = false;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        Reset();
        
        _flightMin = (Vector2)startPos - flightAreaSize / 2f;
        _flightMax = (Vector2)startPos + flightAreaSize / 2f;
        _baseSpeed = speed;
        _targetPatrolPoint = GetRandomPointInBounds();
    }

    void Update()
    {
        if (_isDead) return;

        LoadState();
        
        if (!_mistFormTriggered && health <= monsterdata.health * 0.5f)
        {
            StartCoroutine(MistFormRoutine());
        }
    }
    
    public new void TakeDamage(float amount)
    {
        if (_isInvulnerable) return;
        base.TakeDamage(amount);
    }

    public override void Attack(Collider2D other, int id)
    {
        if (other.CompareTag("Player"))
        {
            BasicControl playerControl = other.GetComponent<BasicControl>();
            if (playerControl == null) return;
            
            float finalDamage = damage;
            bool wasCrit = false;

            if (_nextHitIsMegaCrit)
            {
                finalDamage *= megaCritMultiplier;
                _nextHitIsMegaCrit = false;
                wasCrit = true;
                Debug.Log("<color=red>MEGA CRITICAL HIT!</color>");
            }
            else if (Random.value < critChance)
            {
                finalDamage *= critMultiplier;
                wasCrit = true;
                Debug.Log("Critical Hit!");
            }

            playerControl.TakeDamage(finalDamage);
            
            if (id == 0)
            {
                float healAmount = finalDamage * meleeLeechRate;
                health = Mathf.Min(health + healAmount, monsterdata.health);
                Debug.Log($"Leeched {healAmount} health.");
            }
            
            if (id == 0)
            {
                _isReturning = true;
                currentState = State.Idle;
                Debug.Log("Melee hit confirmed. Returning to start position.");
            }
            
            else if (id == 1)
            {
                SetEffect("SpeedLeech", speedLeechDuration);
                StartCoroutine(SpeedBoostRoutine());
            }
        }
    }

    private IEnumerator SpeedBoostRoutine()
    {
        speed = _baseSpeed * speedLeechMultiplier;
        yield return new WaitForSeconds(speedLeechDuration);
        speed = _baseSpeed;
    }

    private IEnumerator MistFormRoutine()
    {
        _mistFormTriggered = true;
        _isInvulnerable = true;
        
        Debug.Log("Vampire enters Mist Form!");
        
        Color c = _sr.color;
        _sr.color = new Color(c.r, c.g, c.b, 0.4f);
        
        rb.velocity = Vector2.zero;
        Vector3 highPos = transform.position + Vector3.up * mistFormRepositionHeight;
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, highPos, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(mistFormDuration);
        
        _sr.color = new Color(c.r, c.g, c.b, 1f);
        _isInvulnerable = false;
        _nextHitIsMegaCrit = true;
        Debug.Log("Mist Form finished. Next hit will be MEGA CRITICAL.");
    }

    protected override void IdleState(float dist)
    {
        if (_isReturning)
        {
            ReturnToOrigin();
            return;
        }
        
        rb.velocity = new Vector2(Mathf.Cos(Time.time) * 1.5f, Mathf.Sin(Time.time) * 1.5f);

        if (dist < monsterdata.detectRange) currentState = State.Chase;

        idleTimer += Time.deltaTime;
        if (idleTimer >= monsterdata.idleTime)
        {
            idleTimer = 0;
            _targetPatrolPoint = GetRandomPointInBounds();
            currentState = State.Patrol;
        }
    }

    protected override void PatrolState(float dist)
    {
        if (_isReturning)
        {
            ReturnToOrigin();
            return;
        }
        
        if (dist < monsterdata.detectRange)
        {
            currentState = State.Chase;
            return;
        }
        
        Vector2 dir = (_targetPatrolPoint - transform.position).normalized;
        rb.velocity = dir * speed;
        FaceTo(dir.x);

        if (Vector2.Distance(transform.position, _targetPatrolPoint) < 0.5f)
        {
            currentState = State.Idle;
        }
    }

    protected override void ChaseState(float dist)
    {
        if (_isReturning)
        {
            ReturnToOrigin();
            return;
        }
        
        if (dist <= monsterdata.attackRange)
        {
            rb.velocity = Vector2.zero;
            currentState = State.Attack;
            return;
        }
        
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * speed * 1.2f;
        FaceTo(dir.x);
    }

    protected override void AttackState(float dist)
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= monsterdata.attackCooldown)
        {
            attackTimer = 0;
            
            anim.SetTrigger("Melee");
        }
        
        if (isPlayerInRangedZone && playerInZoneCollider != null)
        {
            dpsTimer += Time.deltaTime;

            if (dpsTimer >= 1.0f)
            {
                dpsTimer = 0f;
                
                anim.SetTrigger("Ranged");
                
                Attack(playerInZoneCollider, 1); 
            
                Debug.Log("Periodic Ranged Damage Dealt!");
            }
        }
        
        if (dist > monsterdata.attackRange && !isPlayerInRangedZone) 
        {
            currentState = State.Chase;
        }
    }
    
    private void ReturnToOrigin()
    {
        Vector2 direction = ((Vector2)startPos - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
        FaceTo(direction.x);
        
        if (Vector2.Distance(transform.position, startPos) < 0.5f)
        {
            _isReturning = false;
            rb.velocity = Vector2.zero;
            Debug.Log("Returned to origin. Resuming normal AI.");
        }
    }

    public override void Die()
    {
        if (_isDead) return;
        _isDead = true;
    
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
    
        anim.SetTrigger("Die");
        
        GetComponent<Collider2D>().enabled = false;
        
        Destroy(gameObject, 2f);
    }
    
    private void LoadAnimationState()
    {
        anim.SetBool("isFlying", rb.velocity.magnitude > 0.1f || currentState == State.Idle);
    }
    
    private Vector2 GetRandomPointInBounds()
    {
        return new Vector2(
            Random.Range(_flightMin.x, _flightMax.x),
            Random.Range(_flightMin.y, _flightMax.y)
        );
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(Application.isPlaying ? (Vector3)startPos : transform.position, (Vector3)flightAreaSize);
    }
}