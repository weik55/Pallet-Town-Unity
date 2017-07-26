using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// To Do: make a GameObject called GameFlags when the game first starts, and then doesn't get destoryed on load
// Make sure that GameFlags is a singleton by forcing GameManager to check if GameFlags is already created.
// If this works, should also make the player
// And if the player can be made, then the load level scripts can load coordinates to the game manager (which makes a ton more sense!!!)

public class GameManager : MonoBehaviour {

	private static bool isInstanced = false;
	private static Dictionary<string, int> questFlags;

	// Awake function checks if it's a singleton
	void Awake (){
		if (!isInstanced) {
			questFlags = new Dictionary<string, int> ();
			isInstanced = true;

		} else if (isInstanced) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (this);
	}

	// Checks to see if the GameManager is already keeping track of the quest
	public static bool ContainsQuestline (string questLine){
		return questFlags.ContainsKey (questLine);
	}

	// Adds the specified quest to keep track of, soft error if it already has that quest.
	// Perhaps make this a hard error?
	public static void AddQuestline(string questLine){
		if (!questFlags.ContainsKey (questLine)) {
			questFlags.Add (questLine, 0);
		} else {
			Debug.LogError ("ERROR! Dictionary collision: Same quest detected!" + "Quest: " + questLine);
		}
	}

	// Returns the current event number that the specified quest is at, returns -1 if no quest
	// This could also hard error if we wanted, but probably wiser to return negative int.
	public static int GetCurrentEventNumber (string questLine){
		if (questFlags.ContainsKey (questLine)) {
			return questFlags [questLine];
		} else {
			return -1;
		}
	}

	// Sets current event number of the specified quest
	public static void SetCurrentEventNumber (string questLine, int eventNum){
		if (questFlags.ContainsKey (questLine)) {
			questFlags [questLine] = eventNum;
		} else {
			Debug.LogError ("ERROR! No quest, " + questLine + " found in dictionary to set!");
		}
	}

	// Advances the Questline by one event
	public static void AdvanceQuestline (string questLine){
		if (questFlags.ContainsKey (questLine)) {
			questFlags [questLine] += 1;
		} else {
			Debug.LogError ("ERROR! No quest, " + questLine + " found in dictionary to advance!");
		}
	}

	// Does transition stuff


}
