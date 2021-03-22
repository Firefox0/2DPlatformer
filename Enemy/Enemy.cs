using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float health;

    public void take_damage(float damage)
    {
        health -= damage;
        if (health <= 0) {
            GameObject.Destroy(gameObject);
        }
    }
}
