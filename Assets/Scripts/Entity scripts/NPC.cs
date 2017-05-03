using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

public class NPC : Enemy {
	private bool aggressive = false;
	private EncounterInstance e;
	public override bool takeTurn(bool init){
		if(aggressive)
			base.takeTurn(init);
		return false;
	}

	public void setEncounter(EncounterInstance e){
		this.e = e;
	}

	public void makeAggressive(){
		this.aggressive = true;
	}

	public EncounterInstance getEncounter(){return this.e;}
}
