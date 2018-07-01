using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float moveSpeed = 1;
    public float fieldOfVisionDistance = 25f;
    public float fieldOfVisionAngle = 90f;

    public float attackRange = 1f;
    public float attackRate = 1f;
    public float attackForce = 15f;
    public int attackDamage = 50;

    public float enemySightedStoppingDistance = 2.5f;
    public float walkAroundTimeDelay = 6f;

    public float searchDuration = 15f;
    public float searchingTurnSpeed = 120f;

    [HideInInspector] public float waypointStoppingDistance = 0.5f;
    [HideInInspector] public float alertAlliesRadius = 10f;
}