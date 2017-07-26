using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	private Dictionary<string, int> flags = new Dictionary<string, int> ();

	public void AddFlag (string name) {
		flags.Add (name, 1);
	}

	public void SetFlag (string name, int cut) {
		flags [name] = cut;
	}

	public void IncrementFlag (string name) {
		if (HasFlag (name)) {
			flags [name] += 1;
		}
	}

	public bool HasFlag (string name) {
		return flags.ContainsKey (name);
	}

	public void LoadFlags (Dictionary<string, int> flags) {
		this.flags = flags;
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/* We might use this sometime.
	 * public class Flag {
		private string name;
		private int cut;


		public Flag (string name) {
			this.name = name;
			cut = 0;
		}

		public bool IsSet () {
			if (cut > 0) {
				return true;
			}
			return false;
		}

		public int GetCut () {
			return cut;
		}

		public void Increment () {
			cut = cut + 1;
		}

		public void SetCut (int num) {
			cut = num;
		}
	}

	public Flag GetFlag (string name){
		
	}
	*/
}
