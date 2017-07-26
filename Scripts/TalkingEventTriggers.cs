using UnityEngine;
using System.Collections;

public class TalkingEventTriggers : TalkingObject {

	public string questline;
	public bool isStartTrigger;
	public int lookForEventNumber;
	public bool setQuestline;
	public int eventNumber;

	protected override void Start (){
		base.Start ();
	}

	protected override void NextDialogue () {
		base.NextDialogue ();
		if (!HasNextDialogue ()) {
			if (isStartTrigger && !GameManager.ContainsQuestline(questline)) {
				GameManager.AddQuestline (questline);
			} else if (GameManager.ContainsQuestline(questline) && GameManager.GetCurrentEventNumber (questline) == lookForEventNumber){
				if (setQuestline) {
					GameManager.SetCurrentEventNumber (questline, eventNumber);
				} else {
					GameManager.AdvanceQuestline (questline);
				}
				Debug.Log ("Quest is advancing");
			}
		}
	}
}
