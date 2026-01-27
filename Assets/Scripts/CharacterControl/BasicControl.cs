using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BasicControl : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private bool _isDead = false;

    public GameObject fireballPrefab;   // 拖火球预制体
    public float launchSpeed = 20f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (transform.childCount > 0)
            _sr = transform.GetChild(0).GetComponent<SpriteRenderer>();

    }

    private float _nextAttackTime = 0f;

    void Update()
    {
        if (_isDead) return;

        HandleMovement();

        //HandleJump();

        Attack();

        float h = Input.GetAxis("Horizontal");

        if (h != 0 && _sr != null)
        {
            _sr.flipX = h > 0;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Launch();
        }
    }

    void Launch()
    {
        // 从枪口/手发射
        GameObject fb = Instantiate(fireballPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * launchSpeed;   // 2D：right 发射
        // 3D：rb.velocity = transform.forward * launchSpeed;
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            float h = Input.GetAxis("Horizontal");

            // 使用当前实际移动速度
            _rb.velocity = new Vector2(h * GameDataManager.Instance.moveSpeed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    // Visualization for the Editor to see the ground check circle
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    private void Attack()
    {
        if (Time.time >= _nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Try to perform attack");

                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

                foreach (Collider enemy in hitEnemies)
                {
                    Monster monster = enemy.GetComponent<Monster>();

                    if (monster != null)
                    {
                        monster.TakeDamage(GameDataManager.Instance.damage);
                    }
                }

                _nextAttackTime = Time.time + GameDataManager.Instance.attackCooldown;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        GameDataManager.Instance.health -= damage;
    }
}