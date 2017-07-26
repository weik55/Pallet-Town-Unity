using UnityEngine;
using System.Collections;

public enum Direction {Down = 1, Left, Up, Right};

public class WorldObject : MonoBehaviour {

	protected Renderer rend;

	protected virtual void Awake () {
		rend = GetComponent<Renderer> ();
		SpriteLayerSort ();
	}

	protected void SpriteLayerSort () {
		if (rend != null) {
			float yFactor = transform.position.y * -10;
			rend.sortingOrder = (int)yFactor;
		}
	}
}
