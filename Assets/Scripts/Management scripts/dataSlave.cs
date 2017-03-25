using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

public class dataSlave : MonoBehaviour {

	public bool newGame = false;


	public XElement playerSave, market, slums, entertainment, government, manor, university, temple;

	public Dictionary<string,XElement> areas;

	public Dictionary<string,int> areaNums;

	public static dataSlave instance;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this);
		instance = this;

		areas =  new Dictionary<string, XElement>{
			{"playerSave",playerSave},
			{"Market",market},
			{"Slums",slums},
			{"Entertainment",entertainment},
			{"Government",government},
			{"Manor",manor},
			{"University",university},
			{"Temple", temple}
		};

		areaNums =  new Dictionary<string, int>{
			{"Market",1},
			{"Slums",2},
			{"Entertainment",3},
			{"Government",4},
			{"Manor",5},
			{"University",6},
			{"Temple", 7}
		};
	}

	public void updateEles(){
		market = areas["Market"];
		slums = areas["Slums"];
	}


	public void updateDicts(){
		areas["Market"] = market;
		areas["Slums"] = slums;
	}
}
