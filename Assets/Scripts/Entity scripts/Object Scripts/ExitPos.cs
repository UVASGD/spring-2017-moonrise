using UnityEngine;
using System.Collections;

public class ExitPos : MonoBehaviour {

	private string target;


	public void setTarget(string s){
		target = s;
	}

	public string getTarget(){
		return target;
	}
}
