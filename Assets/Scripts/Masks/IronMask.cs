using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class IronMask : MonoBehaviour
{
    [Header("近战攻击范围")]
    public Collider2D attackcollider;

    [Header("技能冷却时间")]
    public float skillCooldown = 30f;

    [Header("击飞冲刺")]
    public float dashDist = 3f;        // 冲刺距离
    public float dashTime = 0.15f;     // 敌人击退硬直时间
    public float knockBack = 18f;       // 敌人后退速度

    [Header("检测")]
    public LayerMask enemyLayer;

    public float cooldownTimer { get; private set; } = 0;

    private Transform player;


    void Start()
    {
        player = GameDataManager.Instance.player;
    }


    void Update()
    {
        bool attacking = player.GetComponent<BasicControl>().attacking;
        if (Input.GetKeyDown(KeyCode.J) && !attacking)
        {
            player.GetComponent<BasicControl>().attacking = true;
            SimpleAttack();
        }



        if (Input.GetKeyDown(KeyCode.L)&&cooldownTimer==0)
        {
            player.GetComponent<BasicControl>().attacking = true;
            player.GetComponent<Animator>().SetTrigger("HitAttack");
            cooldownTimer = skillCooldown;
            PlayerInfoManager.Instance.SkillCoolDown(skillCooldown);
            StartCoroutine(DashAttack());

        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if(cooldownTimer < 0)
        {
            cooldownTimer = 0;
        }
    }

    void SimpleAttack()
    {
        player.GetComponent<Animator>().SetTrigger("ShortAttack");
        attackcollider.enabled = true;
    }

    public void Attack()
    {
        player.GetComponent<BasicControl>().IronEndAttack();
        var inside = attackcollider.GetComponent<PlayerAttackRange>().GetMonstersInsideNow();
        for (int i = inside.Count - 1; i >= 0; --i)
        {
            var col = inside[i];
            if (col == null) continue;

            var monster = col.GetComponent<Monster>();
            if (monster != null)
                monster.TakeDamage(GameDataManager.Instance.damage);
        }
    }

    IEnumerator DashAttack()
    {
        yield return new WaitForSeconds(0.2f); 
        // 1. 面朝方向
        float dir = Mathf.Sign(player.localScale.x);
        Vector2 start = player.position;
        Vector2 end = start + Vector2.right * dir * dashDist;

        // 2. 瞬移过去（可选动画）
        player.position = end;
        Physics2D.SyncTransforms();

        var hits = attackcollider.GetComponent<PlayerAttackRange>().GetMonstersInsideNow();
        for (int i = hits.Count - 1; i >= 0; --i)
        {
            var col = hits[i];
            if (col == null) continue;

            var monster = col.GetComponent<Monster>();
            if (monster != null)
                monster.TakeDamage(GameDataManager.Instance.damage, dashTime, new Vector2(dir * knockBack, UnityEngine.Random.Range(3,6)));
        }

        // 4. 硬直（后摇）
        yield return new WaitForSeconds(dashTime);

        // 5. 可接后摇动画/特效
    }

}
