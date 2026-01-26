using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BasicControl : MonoBehaviour
{
    [Header("Player Settings")]
    public float baseMoveSpeed = 5f;
    public float baseJumpForce = 7f;
    public float playerDamage = 10f;
    public float attackCooldown = 0.5f;

    [Header("Ground Detection")]
    public Transform groundCheck;    // Assign an empty child object here
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    private Rigidbody _rb;
    private float _currentMoveSpeed;
    private MeshRenderer _sr;
    private bool _isDead = false;
    private bool _isGrounded;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (transform.childCount > 0)
            _sr = transform.GetChild(0).GetComponent<MeshRenderer>();

        _currentMoveSpeed = baseMoveSpeed;
    }

    private float _nextAttackTime = 0f;

    void Update()
    {
        if (_isDead) return;

        CheckGround();

        HandleMovement();

        HandleJump();

        Attack();

        float h = Input.GetAxis("Horizontal");

        //if (h != 0 && _sr != null)
        //{
        //    _sr.flipX = h > 0;
        //}
    }

    void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleJump()
    {
        // Jump only if Space is pressed AND we are touching the ground
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, baseJumpForce, _rb.velocity.z);
        }
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            float h = Input.GetAxis("Horizontal");

            // 使用当前实际移动速度
            _rb.velocity = new Vector2(h * baseMoveSpeed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    // Visualization for the Editor to see the ground check circle
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
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
                        monster.TakeDamage(playerDamage);
                    }
                }

                _nextAttackTime = Time.time + attackCooldown;
            }
        }
    }
}