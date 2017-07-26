using UnityEngine;
using System.Collections;

public class NonPlayerContoller : TalkingObject {
	
	// Movement behavior variables
	public float maxMoveSpeed;
	public Direction startDirection;
	public float minIdleTime;			// Time ranges can help determine pauses in tasks
	public float maxIdleTime;			// or intervals between random instructions
	public bool spinOnIdle;
	public Vector2[] posInstructions;	// Might need to be a list instead of an array
	public bool loopMovement;

	// Final variable for how long an npc timesout when blocked, could probably be just a public var
	protected readonly float blockedTimeout = 5;

	protected Animator animator;
	protected CharacterAnimationController cac;
	protected Vector2 curInstruction;

	protected Direction npcDir;
	protected bool isIdle;
	protected bool isMoving;
	protected bool hasMoveInstruction;
	protected float idleTimer;
	protected float spinDirTimer;
	protected bool hasNextTask;
	protected int instructionIndex;

	// Initializes classes and sets initial variables
	protected override void Start () {
		animator = GetComponent<Animator> ();
		cac = new CharacterAnimationController (animator);

		TurnToDir (startDirection);
		isIdle = false;
		spinDirTimer = minIdleTime;
		instructionIndex = -1;
		isMoving = false;

		LoadTasks ();
		base.Start ();
	}

	// This update function is called at a fixed interval and is not frame speed dependant.
	void FixedUpdate () {
		
		if (isIdle) {
			RunIdle ();
		}

		if (isEngaged) {
			TurnToPlayer ();
		}

		if (hasMoveInstruction && !isMoving && !isEngaged && !isIdle) {			
			NextMove ();
		}

		if (isMoving && !isEngaged && !isIdle) {
			
			// If current instruction is to idle, npc idles
			if (curInstruction.magnitude == 0) {
				idleTimer = Random.Range(minIdleTime, maxIdleTime);
				Idle (idleTimer, true);
				return;
			}

			// Else npc moves, unless its blocked
			Vector2 movement = CalculateMovement (curInstruction);
			if (IsBlocked (movement)) {
				movement = new Vector2 (0, 0);
				Idle (blockedTimeout);
			}
			Move (movement);
			if (movement.y != 0) {
				SpriteLayerSort ();
			}

			// Update our current instruction by how much we've moved this frame
			// Once movement task is done, go to the next task
			curInstruction = curInstruction - movement;	
			if (curInstruction.magnitude <= 0.05) {				//Floating point math, less than 0.05 is "close enough" to zero.
				isMoving = false;
			}
		}
	}

	// Loads intials settings for executing positional tasks
	// Note "NextMove" used to be called in here if posInstructructions.Length > 0
	protected virtual void LoadTasks () {
		if (posInstructions.Length > 0) {
			hasMoveInstruction = true;
		} else {
			hasMoveInstruction = false;
		}
	}

	// Method to queue up next movement task in the posInstructions array
	// hasMoveInstruction and isMoving are redundant booleans I feel, minimize if possible
	protected virtual void NextMove () {
		if (instructionIndex + 1 < posInstructions.Length) {
			instructionIndex = instructionIndex + 1;
			curInstruction = posInstructions [instructionIndex];
			hasMoveInstruction = true;
			isMoving = true;
		} else if (loopMovement) {
			instructionIndex = 0;
			curInstruction = posInstructions [instructionIndex];
			hasMoveInstruction = true;
			isMoving = true;
		} else {
			hasMoveInstruction = false;
			isMoving = false;
			cac.StopWalking ();
		}
	}

	// Helper function that calculates the 2d vector the npc needs to move this frame according to its speed
	protected Vector2 CalculateMovement (Vector2 posVector) {

		float deltaX = 0;
		float deltaY = 0;

		// X movement
		if (posVector.x < 0) {
			deltaX = -1 * maxMoveSpeed;
		} else if (posVector.x > 0) {
			deltaX = maxMoveSpeed;
		}

		// Y movement
		if (posVector.y < 0) {
			deltaY = -1 * maxMoveSpeed;
		} else if (posVector.y > 0) {
			deltaY = maxMoveSpeed;
		}

		return new Vector2 (deltaX, deltaY);
	}

	// Move's the npc the number of units specified by the instruction. Is not contrained by speed.
	protected void Move (Vector2 posInstruction){
		if (posInstruction.magnitude != 0) {
			float moveHorizontal = posInstruction.x;
			float moveVertical = posInstruction.y;

			// Set X animations
			if (moveHorizontal < 0) {
				npcDir = Direction.Left;
			} else if (moveHorizontal > 0) {
				npcDir = Direction.Right;
			}

			// Set Y animations
			if (moveVertical < 0) {
				npcDir = Direction.Down;
			} else if (moveVertical > 0) {
				npcDir = Direction.Up;
			}
	
			cac.WalkInDir (npcDir);
			transform.position = transform.position + (Vector3)posInstruction;
		}
	}

	// Checks if player is blocking the character. (Maybe extend this to check for anything blocking)
	protected bool IsBlocked (Vector2 movement) {
		if (pc != null && isTouching) {
			Vector3 posRelative = pc.transform.position - transform.position;
			if (movement.x > 0 && posRelative.x > 0) {
				return true;
			}
			if (movement.x < 0 && posRelative.x < 0) {
				return true;
			}
			if (movement.y > 0 && posRelative.y > 0) {
				return true;
			}
			if (movement.y < 0 && posRelative.y < 0) {
				return true;
			}
		}
		return false;

	}

	// Helper method to set paremeters of idling
	protected void Idle (float timer, bool nextTaskAfterIdle = false) {
		cac.StopWalking ();
		isIdle = true;
		idleTimer = timer;
		hasNextTask = nextTaskAfterIdle;
	}

	// Idle function that gets called every fixedUpdate when idling
	// While it's idling, spin around for awhile
	// Once idle is done, go finish doing the current task
	protected void RunIdle () {
		idleTimer = idleTimer - Time.deltaTime;
		if (spinOnIdle && !isEngaged) {
			if (spinDirTimer <= 0 && idleTimer > minIdleTime) {
				TurnToDir ((Direction) Random.Range (1, 5));
				spinDirTimer = Random.Range (minIdleTime, 3f);
			} else if (spinDirTimer > 0) {
				spinDirTimer = spinDirTimer - Time.deltaTime;
			}
		}
		if (idleTimer <= 0) {
			isIdle = false;
			if (hasNextTask) {
				NextMove ();
				hasNextTask = false;
				spinDirTimer = minIdleTime;
			}
		}
	}

	// Turn's the npc to the player's direction
	protected virtual void TurnToPlayer (){
		Vector3 posRelative = pc.transform.position - transform.position;
		float magX = Mathf.Abs (posRelative.x);
		float magY = Mathf.Abs (posRelative.y);

		if (magX > magY) {
			if (posRelative.x < 0) {
				TurnToDir (Direction.Left);
			} else if (posRelative.x > 0) {
				TurnToDir (Direction.Right);
			}
		} else if (magX < magY) {
			if (posRelative.y < 0) {
				TurnToDir (Direction.Down);
			} else if (posRelative.y > 0) {
				TurnToDir (Direction.Up);
			}
		}

	}

	// Turn's the npc to the specified direction
	protected void TurnToDir (Direction dir) {
		npcDir = dir;
		cac.TurnToDir (dir);
	}
}
