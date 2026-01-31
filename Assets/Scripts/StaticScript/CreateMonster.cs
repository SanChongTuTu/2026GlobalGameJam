using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "CreateData/Monster", order = 1)]
public class CreateMonster : ScriptableObject
{
    public enum MonsterType
    {
        minion,
        elite,
        boss
    }
    [Header("怪物名称")]
    public string monstername;
    [Header("怪物类别")]
    public MonsterType type;
    [Header("生命")]
    public float health = 100f;
    [Header("攻击")]
    public float damage = 10f;
    [Header("移动速度")]
    public Vector2 speed = new Vector2(8, 15);
    [Header("静止时长")]
    public float idleTime = 2f;
    [Header("索敌范围")]
    public float detectRange = 10f;
    [Header("开始攻击范围")]
    public float attackRange = 1.5f;
    [Header("垂直视野")]
    public float heightDetectRange = 2.5f;   
    [Header("攻击间隔")]
    public float attackCooldown = 2f;
    [Header("单次巡逻最长时长")]
    public float patrolDuration = 2f;
    [Header("是否可以被击飞")]
    public bool canBeKnocked = true;
    [Header("是否可以被硬直")]
    public bool canbeHitStun = true;   
}
