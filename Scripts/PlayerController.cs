using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : WorldObject {

	public static bool isInstanced = false;
	public Text debugXSpeedText;
	public Text debutYSpeedText;
	public float maxSpeed;
	public Vector2 startPosition;
	public Direction startDirection;

	public bool isFrozen { get; set; }
	public Direction playerDirection { get; set; }

	private Rigidbody2D rb2d;
	private Animator animator;
	private CharacterAnimationController cac;

	// Makes sure player is a singleton, sets variables for the player
	protected override void Awake () {
		if (!isInstanced) {
			isInstanced = true;

			DontDestroyOnLoad (this);
			rb2d = GetComponent<Rigidbody2D> ();
			rb2d.freezeRotation = true;
			animator = GetComponent<Animator> ();
			cac = new CharacterAnimationController (animator);

			SceneManager.activeSceneChanged += SetPlayerStart;
		} else if (isInstanced) {
			Destroy (gameObject);
		}

		base.Awake ();
	}


	// Initializes some variables
	void Start () {
		isFrozen = false;
	}


	void FixedUpdate() {
		// Sets animation to standing if player is in "frozen" state
		if (isFrozen) {
			cac.StopWalking ();
		}

		// Checks if the player is frozen, then handles movement controls
		if (!isFrozen) {

			
			float moveHorizontal = Input.GetAxisRaw ("Horizontal");
			float moveVertical = Input.GetAxisRaw ("Vertical");

			// Debug text to check inputs
			//debugXSpeedText.text = "X Speed : " + moveHorizontal.ToString ();
			//debutYSpeedText.text = "Y Speed : " + moveVertical.ToString ();

			// Controls for x movement
			if (moveHorizontal < 0) {
				moveHorizontal = -1 * maxSpeed;
				playerDirection = Direction.Left;
			} else if (moveHorizontal > 0) {
				moveHorizontal = maxSpeed;
				playerDirection = Direction.Right;
			}
				
			// Controls for y movement
			if (moveVertical < 0) {
				moveVertical = -1 * maxSpeed;
				playerDirection = Direction.Down;
			} else if (moveVertical > 0) {
				moveVertical = maxSpeed;
				playerDirection = Direction.Up;
			}

			// Controls animation
			if (moveVertical == 0 && moveHorizontal == 0) {
				cac.StopWalking ();
			} else {
				cac.WalkInDir (playerDirection);
			}

			Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
			if (movement.y != 0) {
				SpriteLayerSort ();
			}

			//testing movement velocities instead of MovePosition function
			//at a fixed update of .02 sec, my previous maxSpeed variable was at 0.125 making the player move 6.25 units per second
			//if instead we set the velocity to 6.25 seconds, we should have the same result... 
			//but maybe the physics is better calculated if there are collisions, and makes unit velocity not frame dependent
			//rb2d.MovePosition (rb2d.position + movement);
			rb2d.velocity = movement;
		}
	}

	// void SetPlayerStart () is the correct implimentation but gives us an indeciferable error for right now, for some reason, we have to have arguments
	void SetPlayerStart (Scene s1, Scene s2) {
		transform.position = (Vector3) startPosition;
		TurnToDir (startDirection);
	}


	// Turn's the npc to the specified direction
	void TurnToDir (Direction dir){
		playerDirection = dir;
		cac.TurnToDir (dir);
	}

	// Cleans up delegates
	void OnDestroy (){
		SceneManager.activeSceneChanged -= SetPlayerStart;
	}
}

