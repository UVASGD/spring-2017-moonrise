using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

public class ResultApply {
	string key;
	int value;
	public ResultApply(string key, int value){
		this.key = key;
		this.value = value;
	}

	public void apply(Player p){
		p.applyEffect(this.key,this.value);
	}

	public bool isAggro(){
		return(this.key == "aggressive");
	}
}
