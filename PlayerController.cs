using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//TODO: 
// - add cooldowns
// - add attacking (object vanishes if destroyed)
// - refactoring
// - super dash after walljumping

public class PlayerController : MonoBehaviour
{
	public float run_speed;
	public float jump_force;           // Amount of force added when the player jumps.
	public float horizontal_direction = 0f;
	public float dash_cooldown = 3;
	const float circle_radius = 0.4f;
	const float movement_smoothing = 0.02f;   // How much to smooth out the movement
	const float walljump_horizontal_force = 7f;
	const float walljump_vertical_force = 15f;
	const float dash_horizontal_force = 70f;
	const float dash_vertical_force = 0f;

	public bool air_control = false;          // Whether or not a player can steer while jumping;
	public bool allowed_to_walljump;
	public bool touching_wall;
	public bool grounded;                    // Whether or not the player is grounded.
	public bool jump = false;
	public bool facing_right = true;          // For determining which way the player is currently facing.
	public bool shift = false;
	public bool left = false;
	public bool right = false;
	public bool dash_ready = true;
	public bool walljumped = false;

	public LayerMask ground_layer;           // A mask determining what is ground to the character
	public LayerMask wall_layer;

	public Transform ground_check;            // A position marking where to check if the player is grounded.
	public Transform ceiling_check;           // A position marking where to check for ceilings
	public Transform wall_check;

	public Rigidbody2D Rigidbody2D;
	public Vector3 Velocity = Vector3.zero;

	Hashtable cooldowns = new Hashtable();

	void Awake() {
		Rigidbody2D = GetComponent<Rigidbody2D>();
	}

    void Start()
    {

    }

    void Update()
    {
		horizontal_direction = Input.GetAxisRaw("Horizontal");
		if (Input.GetKeyDown(KeyCode.Space)) {
			jump = true;
		} else if (Input.GetKeyDown(KeyCode.LeftShift)) {
			shift = true;
		}

		if (Input.GetKey(KeyCode.A)) {
			left = true;
        } else if (Input.GetKey(KeyCode.D)) {
			right = true;
        }
    }

    void FixedUpdate()
	{
		// Handle all physics in this special method.
		grounded = Physics2D.OverlapCircle(ground_check.position, circle_radius, ground_layer);
		handle_general_movement();
		if (grounded && jump) {
			Jump();
		}
		if (shift) {
			Dash(facing_right);
		}
		if (walljumped && grounded) {
			walljumped = false;
        }
		if (allowed_to_walljump) {
			touching_wall = Physics2D.OverlapCircle(wall_check.position, circle_radius, wall_layer);
			handle_wall_interactions();
        }
		jump = false;
		shift = false;
	}

	/*
	void handle_cooldowns()
    {
		dash_timer -= Time.deltaTime;
		if (dash_timer <= 0) {
			dash_ready = true;
        }
    }
	*/

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
		Move(horizontal_direction * run_speed * Time.fixedDeltaTime);
	}

	void handle_wall_interactions()
    {
		if (!touching_wall || grounded) {
			Rigidbody2D.gravityScale = 3f;
			return;
        }
		wallslide();
		walljump(facing_right);
	}

	void wallslide()
    {
		if (Rigidbody2D.velocity.y >= 0) {
			return;
        }
		if (!facing_right && !left || facing_right && !right) {
			return;
        }
		// Slow down if falling from a higher place.
		Rigidbody2D.velocity = new Vector2(0f, Rigidbody2D.velocity.y / 2);

		Rigidbody2D.gravityScale = 1;
		left = false;
		right = false;
	}

	void walljump(bool right)
    {
		if (!jump) {
			return;
        }
		if (facing_right) {
			Rigidbody2D.velocity = new Vector2(walljump_horizontal_force * -1, walljump_vertical_force);
		} else {
			Rigidbody2D.velocity = new Vector2(walljump_horizontal_force, walljump_vertical_force);
		}
		flip();
		walljumped = true;
	}

	void Dash(bool right) 
	{
		float new_dash_horizontal_force = dash_horizontal_force;
		float new_dash_vertical_force = dash_vertical_force;
		// Add more force while airborne to compensate for gravity.
		if (!grounded) {
			new_dash_horizontal_force *= 1.5f;
			new_dash_vertical_force = 5f;
        }
		if (!right) {
			new_dash_horizontal_force *= -1;
        }
		Rigidbody2D.velocity = new Vector2(new_dash_horizontal_force, new_dash_vertical_force);
	}

	void Jump()
    {
		Rigidbody2D.AddForce(new Vector2(0f, jump_force));
	}

	void Move(float move)
	{
		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(move * 10f, Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character
		Rigidbody2D.velocity = Vector3.SmoothDamp(Rigidbody2D.velocity, targetVelocity, ref Velocity, movement_smoothing);
		if (move > 0 && !facing_right || move < 0 && facing_right) {
			flip();
		}
	}

	void flip() 
	{
		facing_right = !facing_right;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
