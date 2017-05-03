using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EncounterManager : MonoBehaviour {

	public GameObject dialoguePrefab, NPCPrefab;
	private GameObject Canvas;
	private GameObject dialogueBox;
	private List<Encounter> encounters = new List<Encounter>();

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



	/// <summary>
	/// Loads all encounters, this will probably be a high runtime function pls run only at boot
	/// </summary>
	public void loadEncounters(){
		var files = Directory.GetFiles(Directory.GetCurrentDirectory()+"\\Encounters");
		foreach (string fileName in files)
		{
			Debug.Log(fileName);
			string[] name = fileName.Split('\\');
			Encounter e = new Encounter("Encounters\\"+name[name.Length-1]);
			encounters.Add(e);
		}
	}


	public EncounterInstance makeEncounter(Vector2 pos){
		//Encounter e = new Encounter("Encounters/testEncounter.xml");
		int encounterIndex = Random.Range(0,encounters.Count);
		Debug.Log(encounterIndex);
		Encounter e = encounters[encounterIndex];
		EncounterInstance eI = new EncounterInstance(e,pos,NPCPrefab, dialogueBox);
		return eI;
	}
}
