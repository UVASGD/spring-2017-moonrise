using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using UnityEngine.UI;
using Completed;

public class Encounter {

	Dictionary<string,Branch> branches = new Dictionary<string,Branch>();

	public Encounter(string eName){
		XElement encounter = XElement.Load(eName);
		List<XElement> data = encounter.Elements().ToList<XElement>();
		//should be flags - type - requirements - dialogue/options
		for(int i = 3; i < data.Count; i ++){
			string key = data[i].Elements().ToList<XElement>()[0].Value;
			Branch b = new Branch(data[i]);
			branches.Add(key,b);
		}

		Debug.Log(branches.Count);
	}


	public int displayBranch(string path, GameObject dialogueBox){
		string[] keys = branches.Keys.ToArray();
		if(branches.ContainsKey(path)){
			Branch b = branches[path];
			dialogueBox.transform.FindChild("BranchText").GetComponent<Text>().text = b.text;

			GameObject op1 = dialogueBox.transform.FindChild("Op1").gameObject;
			GameObject op2 = dialogueBox.transform.FindChild("Op2").gameObject;
			GameObject op3 = dialogueBox.transform.FindChild("Op3").gameObject;

			bool valid;

			if(b.opText.Count > 0){
				op1.GetComponent<Text>().text = b.opText[0];
				op1.GetComponent<optionSelect>().enable();



				//Do ability checks
				valid = true;
				foreach(AbilityCheck a in b.requirements[0]){
					valid = a.check(GameManager.instance.player) & valid;
				}

				if(!valid)
					op1.GetComponent<optionSelect>().disable();
			}
			if(b.opText.Count > 1){
				op2.GetComponent<Text>().text = b.opText[1];
				op2.GetComponent<optionSelect>().enable();
				//Do ability checks
				valid = true;
				foreach(AbilityCheck a in b.requirements[1]){
					valid = a.check(GameManager.instance.player) & valid;
				}

				if(!valid)
					op2.GetComponent<optionSelect>().disable();
			}
			else{
				op2.GetComponent<Text>().text = "";
				op2.GetComponent<optionSelect>().disable();
			}
			if(b.opText.Count > 2){
				op3.GetComponent<Text>().text = b.opText[2];
				op3.GetComponent<optionSelect>().enable();

				//Do ability checks
				valid = true;


				foreach(AbilityCheck a in b.requirements[2]){
					valid = a.check(GameManager.instance.player) & valid;
				}

				if(!valid)
					op3.GetComponent<optionSelect>().disable();
			}else{
				op3.GetComponent<Text>().text = "";
				op3.GetComponent<optionSelect>().disable();
			}

			foreach(ResultApply r in b.results){
				r.apply(GameManager.instance.player);
			}
			return 1;
		}else{
			GameObject.Destroy(dialogueBox);
			return -1;
		}
	}
}
