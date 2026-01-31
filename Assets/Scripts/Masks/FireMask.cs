using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMask : MonoBehaviour
{
    [Header("火球预制体")]
    public GameObject fireballPrefab; 
    [Header("火球发射速度")]
    public float launchSpeed = 8f;
    [Header("普通攻击间隔时间")]
    public float simpleattackinterval = 0.5f;
    [Header("技能环绕数量")]
    public int orbCount = 3;
    [Header("环绕半径")]
    public float radius = 1.5f;
    [Header("环绕速度")]
    public float orbitSpeed = 180f;   // 度/秒
    [Header("环绕时间")]
    public float orbitDuration = 10f;
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

        if (Input.GetKeyDown(KeyCode.L)&&cooldownTimer==0 && PlayerInfoManager.Instance.saveaimtime <= 0 && !GameDataManager.Instance.banL)
        {
            cooldownTimer = skillCooldown;
            PlayerInfoManager.Instance?.SkillCoolDown(skillCooldown, skillCooldown + 5);
            SpawnOrbs();
        }

        if(orbs.Count > 0)
        {
            Orbit();
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
        if(cooldownTimer < 0)
        {
            cooldownTimer = 0;
        }
    }

    void SimpleAttack()
    {
        GameObject fb = Instantiate(fireballPrefab, player.position, player.rotation);
        Rigidbody2D rb = fb.transform.GetComponent<Rigidbody2D>();
        Transform chi = fb.transform;
        chi.localScale = new Vector3(Mathf.Sign(player.localScale.x) * Mathf.Abs(chi.localScale.x), chi.localScale.y, chi.localScale.z);
        rb.velocity = player.right * launchSpeed * Mathf.Sign(player.localScale.x);  
    }

    void SpawnOrbs()
    {
        for (int i = 0; i < orbCount; i++)
        {
            float angle = i * 360f / orbCount;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 pos = player.position + dir * radius;

            GameObject orb = Instantiate(fireballPrefab, pos, Quaternion.identity, player);
            orb.GetComponent<Fireball>().damage = 4;
            orb.GetComponent<Fireball>().ifpenetrate = true;
            orb.GetComponent<Fireball>().ifpenetratewall = true;
            orb.GetComponent<Fireball>().lifeTime = orbitDuration;
            orbs.Add(orb);
        }
    }

    void Orbit()
    {
        float angle = Time.time * orbitSpeed;
        for (int i = orbs.Count - 1; i >= 0; i--)
        {
            if (orbs[i] == null)
            {
                orbs.RemoveAt(i);
            }
        }
        for (int i = 0; i < orbs.Count; i++)
        {
            float a = angle + i * 360f / orbs.Count;
            Vector3 dir = Quaternion.Euler(0, 0, a) * Vector3.right;
            orbs[i].transform.position = player.position + dir * radius;
        }
    }
}
