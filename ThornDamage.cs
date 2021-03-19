using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThornDamage : MonoBehaviour
{
    PlayerHealth player_health;

    void Start()
    {
        player_health = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.gameObject.name.Equals("Knight")) {
            player_health.deal_damage(1);
        }
    }
}
