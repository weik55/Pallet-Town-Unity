using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

	// For placing the camera and setting camera bounds
	public static bool isInstanced = false;
	public GameObject player;
	public Vector2 topLeftBound;
	public Vector2 botRightBound;

	// For transitions
	public bool hasTransition;
	public Material transitionEffectMaterial;
	public Texture transitionTexture;
	public float transitionSpeed;

	// Delegates to call for when the screen transtisions
	public delegate void ScreenTransition ();
	public ScreenTransition runTrans;

	public Text debugCameraPosText;
	private float zPosition;
	private bool cameraIsSet;
	//private bool inTransition;

	private float leftBound;
	private float topBound;
	private float rightBound;
	private float botBound;

	private Vector3 offset;

	void Awake (){
		if (!isInstanced) {
			isInstanced = true;

			SceneManager.activeSceneChanged += UnsetCamera;

		} else if (isInstanced) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start () {
		offset = new Vector3 (0, 0, zPosition);
	}
	
	// LateUpdate is called after all the physics is calculated (I believe)
	// Has camera follow player, until it hits defined boundaries
	void LateUpdate () {
		if (cameraIsSet) {
			float currentX = transform.position.x;
			float currentY = transform.position.y;

			//debugCameraPosText.text = transform.position.ToString ();

			transform.position = player.transform.position + offset;


			if (transform.position.x <= leftBound || transform.position.x >= rightBound) {
				transform.position = new Vector3 (currentX, transform.position.y, transform.position.z);
			}
			if (transform.position.y <= botBound || transform.position.y >= topBound) {
				transform.position = new Vector3 (transform.position.x, currentY, transform.position.z);
			}
		} else {
			SetCamera ();
		}
	}

	// Runs the image that the camera sees through a shader and (in this case) creates a transition effect
	void OnRenderImage (RenderTexture src, RenderTexture dst) {
		if (hasTransition) {
			runTrans ();
			Graphics.Blit (src, dst, transitionEffectMaterial);
		}
	}

	// Sets the camera to where the player is
	void SetCamera(){
		leftBound = topLeftBound.x;
		topBound = topLeftBound.y;
		rightBound = botRightBound.x;
		botBound = botRightBound.y;

		float x = player.transform.position.x;
		float y = player.transform.position.y;
		if (x < leftBound) {
			x = leftBound;
		} else if (x > rightBound) {
			x = rightBound;
		}

		if (y < botBound) {
			y = botBound;
		} else if (y > topBound) {
			y = topBound;
		}

		zPosition = transform.position.z;
		transform.position = new Vector3 (x, y, zPosition);
		//offset = transform.position - player.transform.position;
		offset = new Vector3 (0, 0, zPosition);
		cameraIsSet = true;
	}

	void UnsetCamera (Scene s1, Scene s2) {
		cameraIsSet = false;
	}

	// Runs when the object is destroyed.
	void OnDestroy (){
		SceneManager.activeSceneChanged -= UnsetCamera;
	}
}
