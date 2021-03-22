using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attack_check;
    public PlayerCooldowns player_cooldowns;
    public float attack_range;
    public LayerMask WhatIsDamageable;
    public float attack_damage;

    void Awake()
    {
        player_cooldowns = GameObject.Find("Player").GetComponent<PlayerCooldowns>();
    }

    public void attack_enemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attack_check.position, attack_range, WhatIsDamageable);
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].GetComponent<Enemy>().take_damage(attack_damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack_check.position, attack_range);
    }
}
