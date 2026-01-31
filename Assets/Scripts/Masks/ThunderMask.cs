using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderMask : MonoBehaviour
{
    [Header("雷球预制体")]
    public GameObject fireballPrefab;
    [Header("雷球发射速度")]
    public float launchSpeed = 8f;
    [Header("普通攻击间隔时间")]
    public float simpleattackinterval = 0.5f;

    [Header("雷炮预制体")]
    public GameObject fireball2Prefab;
    [Header("扇面参数")]
    public int bulletCount = 8;     // 一次几发
    public float shootSpeed = 10f;   // 子弹初速度
    public float radius = 0.6f;  // 起点离角色中心的距离（头顶）

    [Header("技能冷却时间")]
    public float skillCooldown = 30f;


    public float cooldownTimer { get; private set; } = 0;
    private List<GameObject> orbs = new List<GameObject>();
    private Transform player;//环绕中心
    private float atktime = -1;

    void Start()
    {
        player = GameDataManager.Instance.player;
    }


    void Update()
    {
        bool attacking = player.GetComponent<BasicControl>().attacking;
        if (Input.GetKeyDown(KeyCode.J) && !attacking)
        {
            atktime = simpleattackinterval;
            player.GetComponent<BasicControl>().attacking = true;
            SimpleAttack();
        }

        if (Input.GetKeyDown(KeyCode.L) && cooldownTimer == 0)
        {
            cooldownTimer = skillCooldown;
            PlayerInfoManager.Instance.SkillCoolDown(skillCooldown);
            Shoot();
        }

        if (atktime > 0)
        {
            atktime -= Time.deltaTime;
        }
        if (atktime <= 0 && atktime > -1)
        {
            player.GetComponent<BasicControl>().attacking = false;
            atktime = -1;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if (cooldownTimer < 0)
        {
            cooldownTimer = 0;
        }
    }

    void SimpleAttack()
    {
        GameObject fb = Instantiate(fireballPrefab, player.position, player.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        rb.velocity = player.right * launchSpeed * Mathf.Sign(player.localScale.x);
    }

    void Skill()
    {
        GameObject fb = Instantiate(fireball2Prefab, player.position, player.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        rb.velocity = player.right * launchSpeed * Mathf.Sign(player.localScale.x);
    }

    public void Shoot()
    {
        // 1. 扇面范围：0° = 左，90° = 正上，180° = 右
        float angleStep = 360f / (bulletCount - 1);   // 等距步长
        float startAngle = 0f;                        // 从左边开始

        for (int i = 0; i < bulletCount; i++)
        {
            // 等距 OR 随机（二选一）
            float angle = startAngle + angleStep * i+Random.Range(-15,16);                 // 等距

            // 2. 方向向量
            Vector2 dir = AngleToDir(angle);

            // 3. 起点：角色中心 + 半径偏移
            Vector2 spawnPos = (Vector2)player.position + dir * radius;

            // 4. 生成子弹
            GameObject bullet = Instantiate(fireball2Prefab, spawnPos, Quaternion.identity);

            // 5. 给速度
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb) rb.velocity = dir * shootSpeed;
        }
    }

    // 角度→方向（2D）
    private Vector2 AngleToDir(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
