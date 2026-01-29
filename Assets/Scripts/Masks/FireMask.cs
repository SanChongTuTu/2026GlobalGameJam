using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMask : MonoBehaviour
{
    [Header("火球预制体")]
    public GameObject fireballPrefab; 
    [Header("火球发射速度")]
    public float launchSpeed = 8f;
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

        if (Input.GetKeyDown(KeyCode.L)&&cooldownTimer==0)
        {
            cooldownTimer = skillCooldown;
            SpawnOrbs();
        }

        if(orbs.Count > 0)
        {
            Orbit();
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
            orb.GetComponent<Fireball>().ifpenetrate = true;
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
