using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

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
		Branch b = this.instanceOf.getBranch(path);
		List<ResultApply> r = b.results[int.Parse(s)-1];
		foreach(ResultApply result in r){
			if(result.isAggro())
				NPCObject.GetComponent<NPC>().makeAggressive();
			else
				result.apply(GameManager.instance.player);
		}
		path = path+s;
		int success = this.instanceOf.displayBranch(path,dialogueBox);
		if(success == -1){
			path = "";
			GameManager.instance.getEnemies().Remove(NPCObject.GetComponent<NPC>());
			GameObject.Destroy(NPCObject);
		}
	}

	public void setDialogueBox(GameObject g){
		this.dialogueBox = g;
	}

}