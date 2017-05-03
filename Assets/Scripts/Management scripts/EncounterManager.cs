using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour {

	public GameObject dialoguePrefab, NPCPrefab;
	private GameObject Canvas;
	private GameObject dialogueBox;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void openEncounter(EncounterInstance e){
		Canvas = GameObject.Find("Canvas");
		dialogueBox = GameObject.Instantiate(dialoguePrefab,Canvas.transform);
		dialogueBox.transform.localPosition = Vector3.zero;
		dialogueBox.transform.FindChild("Op1").GetComponent<optionSelect>().setInstance(e);
		dialogueBox.transform.FindChild("Op2").GetComponent<optionSelect>().setInstance(e);
		dialogueBox.transform.FindChild("Op3").GetComponent<optionSelect>().setInstance(e);
		e.getEncounter().displayBranch("",dialogueBox);

		e.setDialogueBox(dialogueBox);
	}

	public void makeEncounter(){
		Encounter e = new Encounter("Encounters/exampleEncounter.xml");
		EncounterInstance eI = new EncounterInstance(e,new Vector2(60,64),NPCPrefab, dialogueBox);
	}
}
