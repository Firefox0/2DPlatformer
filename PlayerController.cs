using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	public float run_speed;
	public float jump_force;           // Amount of force added when the player jumps.
	public float horizontal_direction = 0f;
	const float circle_radius = 0.2f;
	const float movement_smoothing = 0.02f;   // How much to smooth out the movement
	const float walljump_horizontal_force = 3f;
	const float walljump_vertical_force = 12f;

	public bool air_control = false;          // Whether or not a player can steer while jumping;
	public bool can_walljump;
	public bool grounded;                    // Whether or not the player is grounded.
	public bool jump = false;
	public bool facing_right = true;          // For determining which way the player is currently facing.

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
		horizontal_direction = Input.GetAxis("Horizontal");
		if (Input.GetKeyDown(KeyCode.Space)) {
			jump = Input.GetKeyDown(KeyCode.Space);
        }
    }

    void FixedUpdate()
	{
		grounded = Physics2D.OverlapCircle(ground_check.position, circle_radius, ground_layer);
		if (grounded && jump) {
			Jump();
		}
		if (horizontal_direction != 0 && (grounded || air_control)) {
			Move(horizontal_direction * run_speed * Time.fixedDeltaTime);
		}
		can_walljump = Physics2D.OverlapCircle(wall_check.position, circle_radius, wall_layer);
		if (can_walljump && !grounded && jump) {
			if (facing_right) {
				Rigidbody2D.velocity = new Vector2(walljump_horizontal_force * -1, walljump_vertical_force);
			} else {
				Rigidbody2D.velocity = new Vector2(walljump_horizontal_force, walljump_vertical_force);
			}
		}
		jump = false;
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
			facing_right = !facing_right;
		}
	}

	void flip() 
	{
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
