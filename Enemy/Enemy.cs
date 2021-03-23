using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float health;
    public float damage;
    public bool invincible;

    public void take_damage(float damage)
    {
        if (invincible) {
            return;
        }
        health -= damage;
        if (health <= 0) {
            GameObject.Destroy(gameObject);
        }
    }
}
