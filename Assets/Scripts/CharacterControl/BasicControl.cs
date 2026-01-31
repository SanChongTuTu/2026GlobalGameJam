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
    public float attackslow = 0.5f;//��������˥��
    public float jumpattackslow = 0.7f;//��Ծ��������˥��
    public bool attacking = false;
    
    public SpriteRenderer render;//��Ӱ����

    private Rigidbody2D _rb;
    private bool _isDead = false;
    private Animator _animator;

    private float stoptime = 0;
    private SpriteRenderer[] renderers;
    private List<GameObject> ghosts = new List<GameObject>();
    private bool isTrailing = false;   // ����
    private float spawnInterval;
    private float fadeSpeed;
    private Color ghostColor;

    bool startironattack = false;
    bool startwindattack = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator=GetComponent<Animator>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private float _nextAttackTime = 0f;

    void Update()
    {
        if (_isDead) return;

        if (stoptime>0){
            stoptime-=Time.deltaTime;
            _animator.speed = 0f;
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            _animator.Play(info.shortNameHash, 0, info.normalizedTime);
            return; 
        }
        else
        {
            _animator.speed = 1f;
        }

        if (GetComponentInChildren<WindMask>() != null)
        {
            WindMask one = GetComponentInChildren<WindMask>();
            spawnInterval = one.spawnInterval;
            fadeSpeed = one.fadeSpeed;
            ghostColor = one.ghostColor;
        }

        ChangeMask();

        HandleMovement();

        float h = Input.GetAxis("Horizontal");

        if (h != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(h) * Math.Abs(transform.localScale.x), transform.localScale.y, 1);
        }

        if (startironattack)
        {
            GetComponentInChildren<IronMask>()?.Attack();
        }

        if (startwindattack)
        {
            GetComponentInChildren<WindMask>()?.Attack();
        }
    }

    public void StopAction(float time)
    {
        stoptime = time;
    }

    void ChangeMask()
    {
        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == (int)GameDataManager.Instance.playerType-1);
        }
    }

    void HandleMovement()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            float h = Input.GetAxis("Horizontal");
            if (attacking&&GetComponent<JumpController>().isGrounded)
            {
                h *= attackslow;
            }
            else if (attacking&&!(GetComponent<JumpController>().isGrounded))
            {
                h *= jumpattackslow;
            }
            if (GameDataManager.Instance.playerType == GameDataManager.Type.wind)
            {
                if (transform.GetComponentInChildren<WindMask>().strength)
                {
                    h = Input.GetAxis("Horizontal");
                }
            }
            _animator.SetBool("Move", true);
            _rb.velocity = new Vector2(h * GameDataManager.Instance.moveSpeed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _animator.SetBool("Move", false);
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

    public void EndAttack()
    {
        attacking = false;
    }

    public void IronStartAttack()
    {
        startironattack = true;
    }

    public void WindStartAttack()
    {
        startwindattack=true;
    }

    public void IronEndAttack()
    {
        startironattack = false;
    }

    public void WindEndAttack()
    {
        startwindattack = false;
    }
    /* �ⲿ���ã���ʼ���Ӱ */
    public void StartTrail()
    {
        if (isTrailing) return;
        isTrailing = true;
        InvokeRepeating(nameof(SpawnGhost), 0, spawnInterval);
    }

    /* �ⲿ���ã�ֹͣ���Ӱ */
    public void StopTrail()
    {
        if (!isTrailing) return;
        isTrailing = false;
        CancelInvoke(nameof(SpawnGhost));
        foreach (var g in ghosts) Destroy(g);
        ghosts.Clear();
    }

    void SpawnGhost()
    {
        foreach (var sr in renderers)
        {
            GameObject ghost = new GameObject("Ghost");
            ghost.transform.position = render.transform.position;
            ghost.transform.rotation = render.transform.rotation;
            ghost.transform.localScale = new Vector3(render.transform.localScale.x*Mathf.Sign(transform.localScale.x),render.transform.localScale.y,render.transform.localScale.z);

            SpriteRenderer gSr = ghost.AddComponent<SpriteRenderer>();
            gSr.sprite = sr.sprite;
            gSr.drawMode = sr.drawMode;
            gSr.size = sr.size;       
            gSr.color = ghostColor;
            gSr.sortingOrder = sr.sortingOrder - 1;

            ghosts.Add(ghost);
            StartCoroutine(FadeAndDestroy(ghost, gSr));
        }
    }

    IEnumerator FadeAndDestroy(GameObject ghost, SpriteRenderer gSr)
    {
        Color col = gSr.color;
        while (col.a > 0)
        {
            if (gSr == null) yield break;
            col.a -= GetComponentInChildren<WindMask>() .fadeSpeed * Time.deltaTime;
            gSr.color = col;
            yield return null;
        }
        if (ghost != null) Destroy(ghost);
        ghosts.Remove(ghost);

        Destroy(ghost);
        ghosts.Remove(ghost);
    }

    void OnDestroy() => StopTrail();

    public void TakeDamage(float damage)
    {
        GameDataManager.Instance.health -= damage;
    }
}