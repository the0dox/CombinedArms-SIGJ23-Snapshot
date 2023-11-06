using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemyAI")]
public class EnemyAI : ScriptableObject
{
    [SerializeField] private int startingHealth;
    [SerializeField] private int moveSpeed;
    [SerializeField] private object idle;
    [SerializeField] private EnemyPatrolState patrol;
}
