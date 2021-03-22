using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public int health;

    public void deal_damage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            SceneManager.LoadScene("test");
        }
    }
}
