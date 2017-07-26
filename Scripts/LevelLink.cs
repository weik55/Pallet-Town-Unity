using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLink : MonoBehaviour {

	public string levelToLink;
	public Vector2 playerPositionOnLoad;
	public Direction playerDirectionOnLoad;
	public Vector2 cameraBoundTopLeft;
	public Vector2 cameraBoundBotRight;

	// Method that sets pc coordinates for the next level, as well as camera boundaries
	// Maybe this is something the game manager should be called: gameManager.SetPlayerCoordinates(x, y, dir); gameManager.SetCameraBounds(vector 1, vector 2);
	void OnCollisionEnter2D(Collision2D coll){
		PlayerController pc = coll.gameObject.GetComponent<PlayerController> ();
		CameraController cc = GameObject.Find ("Main Camera").GetComponent<CameraController> ();

		pc.startPosition = (Vector3)playerPositionOnLoad;
		pc.startDirection = playerDirectionOnLoad;

		//transition in - relinquish player control - play player walking in animation
		//set cc.hasTransition = true
		// cc.runTransition += transition. probably set the type controls for the transition here.
			// public bool fadeTranstion
			// public float transitionSpeed
		// How does it know when the transition is done???
			//--when it's done... come back here?
			//-- add level load to the callback?
		//relinquish player control probably is a game manager thing
		SceneManager.LoadScene(levelToLink);

		cc.topLeftBound = cameraBoundTopLeft;
		cc.botRightBound = cameraBoundBotRight;
	}
}
