using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	float current_speed;
	public float run_speed;
	public float crouching_factor;
	public float jump_force;           // Amount of force added when the player jumps.
	public float horizontal_direction = 0f;
	const float circle_radius = 0.4f;
	Vector2 walljump_forces = new Vector2(7f, 15f);
	Vector2 dash_forces = new Vector2(70f, 0f);
	const float movement_smoothing = 0.02f;   // How much to smooth out the movement

	public bool air_control = false;          // Whether or not a player can steer while jumping;
	public bool allowed_to_walljump;
	public bool touching_wall;
	public bool touching_ceiling;
	public bool grounded;                    // Whether or not the player is grounded.
	public bool wants_to_jump = false;
	public bool wants_to_dash = false;
	public bool wants_to_crouch = false;
	public bool crouching = false;
	public bool dash_ready = true;
	public bool walljumped = false;

	public LayerMask ground_layer;           // A mask determining what is ground to the character
	public LayerMask wall_layer;

	public Transform ground_check;            // A position marking where to check if the player is grounded.
	public Transform ceiling_check;           // A position marking where to check for ceilings
	public Transform wall_check;

	public Rigidbody2D Rigidbody2D;
	public Vector3 Velocity = Vector3.zero;

	PlayerCooldowns player_cooldowns;

	void Awake() {
		Rigidbody2D = GetComponent<Rigidbody2D>();
		player_cooldowns = GameObject.Find("Player").GetComponent<PlayerCooldowns>();
		current_speed = run_speed;
	}

    void Update()
    {
		horizontal_direction = Input.GetAxisRaw("Horizontal");
		if (Input.GetKeyDown(KeyCode.Space)) {
			wants_to_jump = true;
		} else if (Input.GetKeyDown(KeyCode.LeftShift)) {
			wants_to_dash = true;
		} else if (Input.GetKeyDown(KeyCode.LeftControl)) {
			wants_to_crouch = true;
        } else if (Input.GetKeyUp(KeyCode.LeftControl)) {
			wants_to_crouch = false;
        }
	}

    void FixedUpdate()
	{
		// Handle all physics in this special method.
		grounded = Physics2D.OverlapCircle(ground_check.position, circle_radius, ground_layer);
		handle_general_movement();
		if (grounded && wants_to_jump) {
			Jump();
		}
		if (wants_to_dash) {
			Dash(facing_right());
		}
		if (walljumped && grounded) {
			walljumped = false;
        }
		if (allowed_to_walljump) {
			touching_wall = Physics2D.OverlapCircle(wall_check.position, circle_radius, wall_layer);
			handle_wall_interactions();
        }
		if (wants_to_crouch && !crouching) {
			Crouch();
			crouching = true;
        } else if (!wants_to_crouch && crouching) {
			touching_ceiling = Physics2D.OverlapCircle(ceiling_check.position, circle_radius, wall_layer);
			if (!touching_ceiling) {
				Stand_up();
				crouching = false;
            }
        }
		wants_to_jump = false;
		wants_to_dash = false;
	}

	void Crouch()
    {
		current_speed = run_speed * crouching_factor;
		gameObject.GetComponent<CircleCollider2D>().radius /= 2;
    }

	void Stand_up()
    {
		current_speed = run_speed;
		gameObject.GetComponent<CircleCollider2D>().radius *= 2;
    }

	bool facing_right()
    {
		if (transform.localScale.x == -1) {
			return false;
        }
		return true;
    }

	void handle_general_movement()
    {
		// Left and right movements.
		if (!grounded && !air_control) {
			return;
        }
		// Don't slow down after walljumping.
		if (walljumped && horizontal_direction == 0) {
			return;
        }
		Move(horizontal_direction * current_speed * Time.fixedDeltaTime);
	}

	void handle_wall_interactions()
    {
		if (!touching_wall || grounded) {
			Rigidbody2D.gravityScale = 3f;
			return;
        }
		wallslide();
		walljump(facing_right());
	}

	void wallslide()
    {
		if (Rigidbody2D.velocity.y >= 0) {
			return;
        }
		if (!facing_right() && horizontal_direction != -1 || facing_right() && horizontal_direction != 1) {
			return;
        }
		// Slow down if falling from a higher place.
		Rigidbody2D.velocity = new Vector2(0f, Rigidbody2D.velocity.y / 2);

		Rigidbody2D.gravityScale = 1;
	}

	void walljump(bool right)
    {
		if (!wants_to_jump) {
			return;
        }
		if (facing_right()) {
			Rigidbody2D.velocity = new Vector2(walljump_forces.x * -1, walljump_forces.y);
		} else {
			Rigidbody2D.velocity = walljump_forces;
		}
		flip();
		walljumped = true;
	}

	void Dash(bool right) 
	{
		if (!player_cooldowns.check_cooldown("Dash")) {
			return;
        }
		Vector2 new_dash_forces = dash_forces;
		// Add more force while airborne to compensate for gravity.
		if (!grounded) {
			new_dash_forces.x *= 1.5f;
			new_dash_forces.y = 5f;
        }
		if (!right) {
			new_dash_forces.x *= -1;
        }
		// TODO: Fix super dash after walljumping properly.
		if (walljumped) {
			new_dash_forces.x /= 7;
		}
		Rigidbody2D.velocity = new_dash_forces;
	}

	void Jump()
    {
		Rigidbody2D.AddForce(new Vector2(0f, jump_force));
	}

	void Move(float move)
	{
		// Move the character by finding the target velocity.
		Vector3 targetVelocity = new Vector2(move * 10f, Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character.
		Rigidbody2D.velocity = Vector3.SmoothDamp(Rigidbody2D.velocity, targetVelocity, ref Velocity, movement_smoothing);
		if (move > 0 && !facing_right() || move < 0 && facing_right()) {
			flip();
		}
	}

	public void flip() 
	{
		Vector3 new_scale = transform.localScale;
		new_scale.x *= -1;
		transform.localScale = new_scale;
	}
}
