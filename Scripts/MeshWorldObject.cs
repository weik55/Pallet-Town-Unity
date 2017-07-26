using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MeshWorldObject : WorldObject {

	public string sortingLayer;
	public int initialOrderInLayer;

	protected override void Awake () {
		rend = GetComponent<Renderer> ();
		SetInitialSortingLayer ();
		SpriteLayerSort ();
	}

	// Sets the order in which a mesh is drawn to work with 2D Sprites
	void SetInitialSortingLayer () {
		if (rend != null) {
			rend.sortingLayerName = sortingLayer;
			rend.sortingOrder = initialOrderInLayer;
		}
	}
}