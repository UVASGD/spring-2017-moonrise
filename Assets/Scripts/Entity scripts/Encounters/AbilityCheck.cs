using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

public class AbilityCheck {
	string ability;
	int value;
	public AbilityCheck(string ability, int value){
		this.ability = ability;
		this.value = value;
	}

	public bool check(Player p){
		int s = p.getSkill(this.ability);
		return s >= this.value;
	}
}
