using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using System.Collections.Generic;

// Currently error's when having a dialogue queue of 0. 
public class TalkingObject : WorldObject {

	//public GameObject panel;				// Setting this directly eliminates having to find it, so could elminate some hidden for loops?
	//public Text panelText;

	public bool readOnlyFromBelow;
	public string[] dialogueTextQueue;		// Consider a list of diaglogue texts for a more generic implementation
	public bool loopDialogue;				// Unsure what happens when npc doesn't have dialogue... we'll think about what we want to happen

	protected GameObject dialogueBox;
	protected Text dialogueText;
	protected GameObject eomImg;
	protected bool isTouching;
	protected bool isEngaged;
	protected int dialogueIndex;
	protected PlayerController pc;

	// Use this for initialization
	protected virtual void Start () {
		dialogueBox = GameObject.Find ("Canvas").transform.FindChild ("Dialogue Box").gameObject;
		isTouching = false;
		isEngaged = false;
		dialogueIndex = -1;
		dialogueText = dialogueBox.GetComponentInChildren<Text> ();
		eomImg = dialogueBox.transform.FindChild ("End of Message Image").gameObject;
	}

	// When collided get's player's info
	protected void OnCollisionEnter2D(Collision2D coll){
		pc = coll.gameObject.GetComponent<PlayerController> ();
		isTouching = true;
	}

	// Sets variables after the collision has exited
	protected void OnCollisionExit2D(){
		isTouching = false;
	}
	
	// Update is called once per frame
	protected void Update () {
		if (isEngaged && Input.GetKeyDown (KeyCode.X)) {
			if (HasNextDialogue ()) {
				NextDialogue ();
				ShowDialogue (CurrentDialogue ());
			} else {
				HideDialogue ();
				pc.isFrozen = false;
				isEngaged = false;
			}
			return;
		}
		// Functions regarding reading the text
		if (isTouching && Input.GetKeyDown (KeyCode.X) && IsFacingMe()) {
			if ((readOnlyFromBelow && pc.playerDirection == Direction.Up) || !readOnlyFromBelow) {
				NextDialogue ();
				if (dialogueIndex == -1) {
					return;
				}
				ShowDialogue (CurrentDialogue ());
				pc.isFrozen = true;
				isEngaged = true;					// Perhaps set this in the ShowDialogue function
			}										// otherwise, dialogue box won't dissappear when called from seperate function;
		}											// caveat, could have a seperate "Close Dialogue" function. good for timed dialogues.
	}

	// Replaces strings to actual symbols in the text
	private string InsertSymbols (string text) {
		text = text.Replace ("<br>", "\n");
		text = text.Replace ("Pokemon", "Pokémon");
		return text;
	}

	// Shows the text in the dialogue box
	protected void ShowDialogue (string text){
		dialogueBox.SetActive (true);
		dialogueText.text = InsertSymbols (text);

		if (!HasNextDialogue ()) {
			eomImg.gameObject.SetActive (true);
		} else {
			eomImg.gameObject.SetActive (false);
		}
	}

	// Hides the dialogue box
	protected void HideDialogue (){
		dialogueBox.SetActive (false);
	}

	// Gets the current dialogue, simple function that can be over written
	protected string CurrentDialogue () {
		return dialogueTextQueue [dialogueIndex];
	}

	// Increases the dialogueIndex if there is more dialogue or if dialogue needs to loop
	protected virtual void NextDialogue () {
		if (HasNextDialogue ()) {
			dialogueIndex = dialogueIndex + 1;
		} else if (loopDialogue) {
			dialogueIndex = 0;
		}
	}

	// Checks to see if there is more dialogue
	protected virtual bool HasNextDialogue (){
		if (dialogueIndex + 1 < dialogueTextQueue.Length) {
			return true;
		}
		return false;
	}

	// Function that returns true if player is facing this object
	// Can be generalized to tell if any object is facing this object with a parameter
	protected bool IsFacingMe () {
		Vector3 offset = transform.position - pc.transform.position;

		//Debug.Log (offset);
		double maxDifferential = 0.5;

		// If player is standing to the left or right of this object
		if (-1 * maxDifferential <= offset.y && offset.y <= maxDifferential) {
			if (offset.x > 0 && pc.playerDirection == Direction.Right) {
				return true;
			}
			if (offset.x < 0 && pc.playerDirection == Direction.Left) {
				return true;
			}
		}
		// If player is standing to the top or bottom of this object
		if (-1 * maxDifferential <= offset.x && offset.x <= maxDifferential) {
			if (offset.y > 0 && pc.playerDirection == Direction.Up) {
				return true;
			}
			if (offset.y < 0 && pc.playerDirection == Direction.Down) {
				return true;
			}
		}

		return false;
	}
}
