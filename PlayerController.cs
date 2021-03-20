using UnityEngine;
using UnityEngine.Events;

//TODO: 
// - air control and walljumping does not work if combined
// - add attacking (object vanishes if destroyed)

public class PlayerController : MonoBehaviour
{
	public float run_speed;
	public float jump_force;           // Amount of force added when the player jumps.
	public float horizontal_direction = 0f;
	const float circle_radius = 0.4f;
	const float movement_smoothing = 0.02f;   // How much to smooth out the movement
	const float walljump_horizontal_force = 7f;
	const float walljump_vertical_force = 15f;
	const float dash_horizontal_force = 70f;
	const float dash_vertical_force = 0f;

	public bool air_control = false;          // Whether or not a player can steer while jumping;
	public bool allowed_to_walljump;
	public bool can_walljump;
	public bool grounded;                    // Whether or not the player is grounded.
	public bool jump = false;
	public bool facing_right = true;          // For determining which way the player is currently facing.
	public bool dash = false;

	public LayerMask ground_layer;           // A mask determining what is ground to the character
	public LayerMask wall_layer;

	public Transform ground_check;            // A position marking where to check if the player is grounded.
	public Transform ceiling_check;           // A position marking where to check for ceilings
	public Transform wall_check;

	public Rigidbody2D Rigidbody2D;
	public Vector3 Velocity = Vector3.zero;


	void Awake() {
		Rigidbody2D = GetComponent<Rigidbody2D>();
	}

    void Update()
    {
		horizontal_direction = Input.GetAxisRaw("Horizontal");
		if (Input.GetKeyDown(KeyCode.Space)) {
			jump = true;
        } else if (Input.GetKeyDown(KeyCode.LeftShift)) {
			dash = true;
		}
    }

    void FixedUpdate()
	{
		grounded = Physics2D.OverlapCircle(ground_check.position, circle_radius, ground_layer);
		if (grounded && jump) {
			Jump();
		}
		if (grounded || air_control) {
			Move(horizontal_direction * run_speed * Time.fixedDeltaTime);
			if (dash) {
				Dash(facing_right);
            }
		}
		if (allowed_to_walljump) {
			can_walljump = Physics2D.OverlapCircle(wall_check.position, circle_radius, wall_layer);
			if (can_walljump && !grounded && jump) {
				if (facing_right) {
					Rigidbody2D.velocity = new Vector2(walljump_horizontal_force * -1, walljump_vertical_force);
				} else {
					Rigidbody2D.velocity = new Vector2(walljump_horizontal_force, walljump_vertical_force);
				}
				flip();
			}
        }
		jump = false;
		dash = false;
	}

	void Dash(bool right) 
	{
		float new_dash_horizontal_force = dash_horizontal_force;
		float new_dash_vertical_force = dash_vertical_force;
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
