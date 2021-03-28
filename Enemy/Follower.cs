using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{

    PlayerSwap player_info;
    public float vision_range;
    Rigidbody2D enemy_rigidbody;
    public float speed;
    Vector2 right_vector;
    Vector2 left_vector;

    string current_character = "";
    GameObject current_character_object;

    void Awake() 
    {
        player_info = GameObject.Find("Player").GetComponent<PlayerSwap>();
        enemy_rigidbody = GetComponent<Rigidbody2D>();
        right_vector = new Vector2(speed, 0f);
        left_vector = new Vector2(-speed, 0f);
    }

    void Start()
    {
        current_character_object = player_info.get_current_object();
    }

    void Update()
    {
        if (!this.current_character.Equals(player_info.current_character)) {
            this.current_character = player_info.current_character;
            current_character_object = player_info.get_current_object();
        }
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, current_character_object.transform.position);
        if (distance <= vision_range) {
            move_towards_player(current_character_object);
        }    
    }

    void move_towards_player(GameObject current_player)
    {
        if (transform.position.x < current_player.transform.position.x) {
            enemy_rigidbody.velocity = right_vector;
        } else if (transform.position.x > current_player.transform.position.x) {
            enemy_rigidbody.velocity = left_vector;
        }
    }
}
