using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public float health;
    public Transform character_check;
    public float character_radius;
    public LayerMask WhatDealsDamage;

    void Update()
    {
        check_collisions();
    }

    public void take_damage(float damage)
    {
        health -= damage;
        if (health <= 0) {
            SceneManager.LoadScene("test");
        }
    }

    void check_collisions()
    {
        Collider2D collider = Physics2D.OverlapCircle(character_check.position, character_radius, WhatDealsDamage);
        if (!collider) {
            return;
        }
        float damage = collider.GetComponent<Enemy>().damage;
        take_damage(damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(character_check.position, character_radius);
    }
}
