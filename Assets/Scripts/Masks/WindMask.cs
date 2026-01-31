using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class WindMask : MonoBehaviour
{
    [Header("近战攻击范围")]
    public Collider2D attackcollider;
    [Header("近战攻击范围2")]
    public Collider2D attackcollider2;

    [Header("技能冷却时间")]
    public float skillCooldown = 30f;

    [Header("残影参数")]
    public float spawnInterval = 0.05f; // 多久留一个影
    public float fadeSpeed = 5f;        // 淡出速度
    public Color ghostColor = new Color(1, 1, 1, 0.6f);
    public float keeptime = 10;
    public bool strength = false;
    private float savespeed;
    private float savedamage;

    [Header("检测")]
    public LayerMask enemyLayer;

    public float cooldownTimer { get; private set; } = 0;
    private float waittime = -1;

    private Transform player;


    void Start()
    {
        player = GameDataManager.Instance.player;
    }


    void Update()
    {
        bool attacking = player.GetComponent<BasicControl>().attacking;
        if (Input.GetKeyDown(KeyCode.J)&&!attacking)
        {
            player.GetComponent<BasicControl>().attacking = true;
            SimpleAttack();
        }



        if (Input.GetKeyDown(KeyCode.L) && cooldownTimer == 0)
        {
            cooldownTimer = skillCooldown;
            PlayerInfoManager.Instance.SkillCoolDown(skillCooldown);
            player.GetComponent<BasicControl>().StartTrail();
            waittime = keeptime;
            strength = true;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if (cooldownTimer < 0)
        {
            cooldownTimer = 0;
        }

        if (waittime > 0)
        {
            waittime -= Time.deltaTime;
        }
        if (waittime <= 0&&waittime!=-1)
        {
            waittime = -1;
            player.GetComponent<BasicControl>().StopTrail();
            strength = false;
        }
    }

    public void SimpleAttack()
    {
        if(strength == true)
        {
            player.GetComponent<Animator>().SetTrigger("HitAttack");
            attackcollider2.enabled = true;
        }
        else
        {
            player.GetComponent<Animator>().SetTrigger("ShortAttack");
            attackcollider.enabled = true;
        }
    }

    public void Attack()
    {
        player.GetComponent<BasicControl>().WindEndAttack();
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

}
