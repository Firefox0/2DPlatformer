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
	public float jump_force;     
	public float horizontal_direction = 0f;
	const float circle_radius = 0.4f;
	Vector2 walljump_forces = new Vector2(7f, 15f);
	Vector2 dash_forces = new Vector2(70f, 0f);
	const float movement_smoothing = 0.02f;

	public bool air_control = false; 
	public bool allowed_to_walljump;
	public bool touching_wall;
	public bool touching_ceiling;
	public bool grounded;                
	public bool wants_to_jump = false;
	public bool wants_to_dash = false;
	public bool wants_to_crouch = false;
	public bool crouching = false;
	public bool dash_ready = true;
	public bool walljumped = false;
	public bool wallsliding = false;

	public LayerMask ground_layer;
	public LayerMask wall_layer;

	public Transform ground_check;
	public Transform ceiling_check;
	public Transform wall_check;

	Rigidbody2D player_rigidbody;
	CircleCollider2D player_collider;
	public Vector3 Velocity = Vector3.zero;

	PlayerCooldowns player_cooldowns;

	void Awake() {
		player_rigidbody = GetComponent<Rigidbody2D>();
		player_cooldowns = GameObject.Find("Player").GetComponent<PlayerCooldowns>();
		current_speed = run_speed;
		player_collider = GetComponent<CircleCollider2D>();
	}

	void Update()
	{
		horizontal_direction = Input.GetAxisRaw("Horizontal");
		if (Input.GetKeyDown(KeyCode.Space)) {
			wants_to_jump = true;
        }
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			wants_to_dash = true;
        }
		wants_to_crouch = Input.GetKey(KeyCode.LeftControl);
	}

	void FixedUpdate()
	{
		// Handle all physics in this special method.
		
		handle_movements();
	}

	void handle_movements() {
		grounded = Physics2D.OverlapCircle(ground_check.position, circle_radius, ground_layer);

		// Left and right movement. Don't slow down after walljumping.
		if (grounded || air_control &&
			!walljumped || horizontal_direction != 0) {
			move(horizontal_direction * current_speed);;
		}

		if (grounded && wants_to_jump) {
			jump();
		}
		if (wants_to_dash) {
			dash(facing_right());
		}

		if (walljumped && grounded) {
			walljumped = false;
		}
		handle_wall_interactions();

		if (wants_to_crouch && !crouching) {
			crouch();
			crouching = true;
		} else if (!wants_to_crouch && crouching) {
			touching_ceiling = Physics2D.OverlapCircle(ceiling_check.position, circle_radius, wall_layer);
			if (!touching_ceiling) {
				stand_up();
				crouching = false;
			}
		}

		wants_to_jump = false;
		wants_to_dash = false;
	}

	void crouch()
	{
		current_speed = run_speed * crouching_factor;
		player_collider.radius /= 2;
	}

	void stand_up()
	{
		current_speed = run_speed;
		player_collider.radius *= 2;
	}

	bool facing_right()
	{
		if (transform.localScale.x == -1) {
			return false;
		}
		return true;
	}

	void handle_wall_interactions()
	{
		touching_wall = Physics2D.OverlapCircle(wall_check.position, circle_radius, wall_layer);
		if (!touching_wall || grounded) {
			player_rigidbody.gravityScale = 3f;
			wallsliding = false;
			return;
		}
		wallslide();
		if (wants_to_jump) {
			walljump(facing_right());
		}
	}

	void wallslide()
	{
		if (wallsliding ||
			player_rigidbody.velocity.y >= 0 || 
			!facing_right() && horizontal_direction != -1 ||
			facing_right() && horizontal_direction != 1) {
			return;
		}
		player_rigidbody.gravityScale = 0.25f;
		player_rigidbody.velocity = new Vector2(0, 0);
		wallsliding = true;
	}

	void walljump(bool right)
	{
		if (!allowed_to_walljump) {
			return;
		}
		if (facing_right()) {
			player_rigidbody.velocity = new Vector2(walljump_forces.x * -1, walljump_forces.y);
		} else {
			player_rigidbody.velocity = walljump_forces;
		}
		flip();
		walljumped = true;
	}

	void dash(bool right)
	{
		if (!player_cooldowns.check_cooldown("Dash")) {
			return;
		}
		Vector2 new_dash_forces = dash_forces;
		if (!right) {
			new_dash_forces.x *= -1;
		}
		player_rigidbody.velocity = new_dash_forces;
	}

	void jump()
	{
		player_rigidbody.AddForce(new Vector2(0f, jump_force));
	}

	void move(float speed)
	{
		// Move the character by finding the target velocity.
		Vector3 targetVelocity = new Vector2(speed, player_rigidbody.velocity.y);
		// And then smoothing it out and applying it to the character.
		player_rigidbody.velocity = Vector3.SmoothDamp(player_rigidbody.velocity, targetVelocity, ref Velocity, movement_smoothing);
		if (speed > 0 && !facing_right() || speed < 0 && facing_right()) {
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
