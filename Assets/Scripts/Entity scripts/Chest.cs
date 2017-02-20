using UnityEngine;
using System.Collections.Generic;
using ItemSpace;

namespace Completed {
	public class Chest : Character {
		private Transform playerTransform;
		private Item item; //item contained in chest;
		// Use this for initialization
		void Start () {
			CurrentHP = 1;
			playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
			item = Item.RandomItem ();
		}

	
		// Update is called once per frame
		void Update () {
			// if(hp == 0) {
			if (playerTransform.position.x == this.transform.position.x && playerTransform.position.y == this.transform.position.y) {
				ObtainItem (playerTransform.GetComponent<Player>()); // this should never happen
			}
		}

		public void ObtainItem(Player player) {
			AddItem (item);
			GameManager.instance.print ("A " + item.Name + " was added to inventory");
			Destroy (this.gameObject);
		}
	}
}