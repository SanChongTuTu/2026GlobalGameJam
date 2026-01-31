using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DeathMask : MonoBehaviour
{
    [Header("冰球预制体")]
    public GameObject iceballPrefab;   
    [Header("冰球发射速度")]
    public float launchSpeed = 8f;
    [Header("技能冷却时间")]
    public float skillCooldown = 30f;

    [Header("激光长度")]
    public float range = 10f;           // 射线长度
    [Header("激光持续时间")]
    public float duration = 10f;         // 激光持续时间
    [Header("激光采样步长")]
    public float sampleStep = 0.1f;          // 采样步长（越小越准）
    [Header("敌人")]
    public LayerMask targetLayer;                  // 敌人层
    public LayerMask obstacleLayer;                // 障碍物层（无法穿透）
    [Header("每秒造成伤害")]
    public float damagePerSec = 1.5f;           // 每秒伤害
    [Header("多长时间造成一次伤害")]
    public float damageInterval = 0.01f;        // 伤害间隔（越小越准）

    [Header("美术材质")]
    public Material laserMaterial;               // 拖红色材质
    public float lineWidth = 0.1f;            // 激光宽度
    public float cooldownTimer { get; private set; } = 0;

    private LineRenderer line;
    private float damageTimer = 0f;
    private float durationtime = 0f;
    private Transform player;//环绕中心


    void Start()
    {
        player = GameDataManager.Instance.player;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SimpleAttack();
        }

        if (Input.GetKeyDown(KeyCode.L)&&cooldownTimer==0 && PlayerInfoManager.Instance.saveaimtime <= 0&&!GameDataManager.Instance.banL)
        {
            cooldownTimer = skillCooldown;
            PlayerInfoManager.Instance.SkillCoolDown(skillCooldown,skillCooldown+5);
            Resetskill();
            StartCoroutine(CastLaser());
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
        GameObject fb = Instantiate(iceballPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * launchSpeed * Mathf.Sign(transform.localScale.x);  
    }

    void Resetskill()
    {
        if (GetComponent<LineRenderer>() == null)
        {
            line = transform.AddComponent<LineRenderer>();
        }
        else { line = GetComponent<LineRenderer>(); }
        line.material = laserMaterial;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
    }

    IEnumerator CastLaser()
    {
        durationtime = duration;
        damageTimer = 0f;

        // 开始画线
        StartCoroutine(DrawLaser());
        yield return new WaitForSeconds(duration);
        // 激光结束
        line.positionCount = 0;
        durationtime = 0f;
        Destroy(line);
    }

    IEnumerator DrawLaser()
    {
        damageTimer = 0f;
        HashSet<Collider2D> hitThisInterval = new HashSet<Collider2D>();

        while (durationtime > 0)
        {
            durationtime -= Time.deltaTime;
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                hitThisInterval.Clear();
            }

            // 每帧重新算起点和方向
            Vector3 start = transform.position;
            Vector3 dir = transform.right * Mathf.Sign(transform.localScale.x);

            // 逐点采样
            List<Vector3> points = new List<Vector3>();
            for (float d = 0; d <= range; d += sampleStep)
            {
                Vector3 pos = start + dir * d;
                points.Add(pos);

                // 障碍物检测（无法穿透）
                Collider2D obstacle = Physics2D.OverlapCircle(pos, sampleStep * 0.5f, obstacleLayer);
                if (obstacle != null)
                {
                    break; // 遇到障碍物，停止射线
                }

                foreach (var hit in Physics2D.OverlapCircleAll(pos, sampleStep * 0.5f, targetLayer))
                {
                    if (hitThisInterval.Contains(hit)) continue;
                    hitThisInterval.Add(hit);
                    hit.GetComponent<Monster>()?.TakeDamage(damagePerSec * damageInterval);
                    hit.GetComponent<Monster>()?.SetEffect("Slow", 5);
                }
            }

            // 画激光（GPU 画线）
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());

            yield return null; // 每帧更新
        }
        // 激光结束
        if(line!=null) 
        line.positionCount = 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 origin = transform.position;
        Vector2 dir = transform.right * Mathf.Sign(transform.localScale.x);
        Gizmos.DrawRay(origin, dir * range);
    }
}
