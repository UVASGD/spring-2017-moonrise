using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;
using System.Xml;

public class Branch {
	public string path = "", text;
	public List<string> opText = new List<string>();
	public List<List<AbilityCheck>> requirements = new List<List<AbilityCheck>>();
	public List<ResultApply> results = new List<ResultApply>();
	//branch XML structure goes text-option-option-option
	public Branch(XElement branch){
		for(int i = 0; i < 3; i++){
			requirements.Add(new List<AbilityCheck>());
		}
		List<XElement> branchData = new List<XElement>(branch.Elements());
		path = branchData[0].Value;
		text = branchData[1].Value;
		//Read through branch results and dialogue options
		int optionNum = 0;
		for(int i = 2; i < branchData.Count; i++){
			XElement b = branchData[i];
			if(b.Name.ToString().Equals("option")){
				//option number - requirements - text
				List<XElement> branchInfo = b.Elements().ToList<XElement>();
				opText.Add(branchInfo[2].Value);
				string[] requirement = branchInfo[1].Value.Split('_');
				foreach(string r in requirement){
					string[] req = r.Split(':');
					if(req.Count() > 1){
						requirements[optionNum].Add(new AbilityCheck(req[0],int.Parse(req[1])));
					}
				}
				optionNum++;
			}else if(b.Name.ToString().Equals("result")){
				string[] effects = b.Value.Split('_');
				foreach(string e in effects){
					string[] effect = e.Split(':');
					if(effect.Count() > 1){
						results.Add(new ResultApply(effect[0],int.Parse(effect[1])));
					}
				}
			}
		}

	}


	public string getPath(){return path;}
}
