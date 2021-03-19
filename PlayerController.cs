using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	public float jump_force = 500f;           // Amount of force added when the player jumps.
	public float movement_smoothing = .02f;   // How much to smooth out the movement
	public bool air_control = false;          // Whether or not a player can steer while jumping;
	public LayerMask ground_layer;           // A mask determining what is ground to the character
	public LayerMask wall_layer;
	public Transform ground_check;            // A position marking where to check if the player is grounded.
	public Transform ceiling_check;           // A position marking where to check for ceilings
	public Transform wall_check;
	public bool can_walljump;

	const float circle_radius = .2f;
	public bool grounded;                    // Whether or not the player is grounded.
	public Rigidbody2D Rigidbody2D;
	public bool facing_right = true;          // For determining which way the player is currently facing.
	public Vector3 Velocity = Vector3.zero;

	public bool wasCrouching = false;

	float horizontal_direction = 0f;
	public float run_speed = 100f;
	bool jump = false;

	void Awake() {
		Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		horizontal_direction = Input.GetAxis("Horizontal");
		if (Input.GetButtonDown("Jump")) {
			jump = true;
        }

		can_walljump = Physics2D.OverlapCircle(wall_check.position, .2f, wall_layer);
		if (can_walljump && !grounded && Input.GetKeyDown("space")) {
			flip();
			Rigidbody2D.velocity = new Vector2(-1 * horizontal_direction * 5f, 10f);
        }
	}

	void FixedUpdate()
	{
		Move(horizontal_direction * run_speed * Time.fixedDeltaTime, false, jump);
		jump = false;

		bool wasGrounded = grounded;
		grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(ground_check.position, circle_radius, ground_layer);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				grounded = true;
            }
		}
	}

	public void Move(float move, bool crouch, bool jump)
	{
		if (grounded || air_control) {

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			Rigidbody2D.velocity = Vector3.SmoothDamp(Rigidbody2D.velocity, targetVelocity, ref Velocity, movement_smoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facing_right || move < 0 && facing_right) {
				flip();
			}
		}

		if (grounded && jump) {
			grounded = false;
			Rigidbody2D.AddForce(new Vector2(0f, jump_force));
		}
	}

	void flip() 
	{
		// Switch the way the player is labelled as facing.
		facing_right = !facing_right;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
