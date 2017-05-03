using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Completed;

public class optionSelect : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler {
	EncounterInstance instance;
	private bool disabled = false;
	public string optionNum = "";

	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerEnter(PointerEventData data){
		if(this.disabled)
			return;
		this.GetComponent<Text>().fontSize = 25;
	}

	public void OnPointerExit(PointerEventData data){
		this.GetComponent<Text>().fontSize = 20;
	}

	public void OnPointerDown(PointerEventData data){
		if(this.GetComponent<Text>().text == "" || this.disabled)
			return;
		this.instance.choice(this.optionNum);
		GameManager.instance.player.UpdateText();
	}

	public void setInstance(EncounterInstance e){
		instance = e;
	}

	public void disable(){
		disabled = true;
		this.GetComponent<Text>().color = Color.gray;
	}

	public void enable(){
		disabled = false;
		this.GetComponent<Text>().color = Color.white;
	}

	public bool isEnabled(){return disabled;}
}
