using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundPlatform : Enemy
{
    public Transform ground_check;
    public bool grounded;
    public LayerMask what_is_ground;
    public float speed;
    float ground_check_radius = 0.2f;
    Rigidbody2D enemy_rigidbody;
    BoxCollider2D enemy_collider;

    void Awake() {
        enemy_rigidbody = GetComponent<Rigidbody2D>(); 
        enemy_collider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {
        grounded = Physics2D.OverlapCircle(ground_check.position, ground_check_radius, what_is_ground);
        if (!grounded) {
            reposition();
        }
        move_forward();
    }

    void reposition() {
        Vector3 new_position = (enemy_collider.size + enemy_collider.offset) * transform.localScale;
        float temp_x = new_position.x;
        new_position.x = new_position.y;
        new_position.y = -temp_x;
        transform.Translate(new_position, Space.Self);
        transform.Rotate(0f, 0f, -90f, Space.Self);
    }

    void move_forward() {
        transform.Translate(speed / 500f, 0f, 0f, Space.Self);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ground_check.position, ground_check_radius);
    }
}
