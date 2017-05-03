
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;

public class StartGame : MonoBehaviour {

	static int continueArea;

	void Start(){
		File.Delete("save.xml");
		//load();
	}

    public void ChangeScene (int change)
    {
		File.Delete("save.xml");
		dataSlave.instance.newGame = true;
        SceneManager.LoadScene("Market");
    }

	public void continueGame (int change)
	{
		dataSlave.instance.newGame = false;
		SceneManager.LoadScene(StartGame.continueArea);
	}

	static public void load(){
		string[] files = null;
		files = Directory.GetFiles(Directory.GetCurrentDirectory());
		foreach (string fileName in files)
		{
			string[] name = fileName.Split('\\');
			if (name.Length == 1)
				name = fileName.Split ('/');
			//Debug.Log (name [name.Length - 1]);
			if(name[name.Length-1] == "save.xml" && !dataSlave.instance.newGame){
				XElement doc = XElement.Load(fileName);

				//curarea - 0
				//player - 1
				//market - 2
				//slums - 3
				//government - 4
				//entertainment - 5
				//manor - 6
				//university - 7
				//temple - 8
				List<XElement> info = doc.Elements().ToList<XElement>();
				foreach(XElement i in info){
					print(i.ToString());
				}
				dataSlave.instance.curLoc = info[0];
				Debug.Log (dataSlave.instance.curLoc);
				dataSlave.instance.playerSave = info[1];
				dataSlave.instance.market = info[2];
				if(info.Count > 3)
					dataSlave.instance.slums = info[3];
				/*dataSlave.instance.government = info[4];
				dataSlave.instance.entertainment = info[5];
				dataSlave.instance.manor = info[6];
				dataSlave.instance.university = info[7];
				dataSlave.instance.temple = info[8];
				*/	
				continueArea = dataSlave.instance.areaNums[dataSlave.instance.curLoc.Value];
				dataSlave.instance.updateDicts();
			}
		}
	}
}
