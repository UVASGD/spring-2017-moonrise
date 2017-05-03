using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterInstance {
	private Encounter instanceOf;
	private string path = "";
	public GameObject NPCObject, dialogueBox;
	public EncounterInstance(Encounter e, Vector2 loc, GameObject NPCPrefab, GameObject DialogueBox){
		instanceOf = e;
		this.dialogueBox = DialogueBox;
		NPCObject = GameObject.Instantiate(NPCPrefab);
		NPCObject.transform.localPosition = new Vector3(loc.x,loc.y,0);
		NPCObject.GetComponent<NPC>().setEncounter(this);

	}

	public Encounter getEncounter(){return instanceOf;}

	public void choice(string s){
		path = path+s;
		int success = this.instanceOf.displayBranch(path,dialogueBox);
		if(success == -1)
			path = "";
	}

	public void setDialogueBox(GameObject g){
		this.dialogueBox = g;
	}
}