using UnityEngine;
using System.Collections.Generic;
using ItemSpace;

namespace Completed {
	public class Chest : Character {
		private Transform playerTransform;
		public Item item; //item contained in chest;
		// Use this for initialization
		void Start () {
			CurrentHP = 1;
			playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
			//item = Item.RandomItem ();
			//replaced by call at destruction.
		}

	
		// Update is called once per frame
		void Update ()
		{
			
		}
	}
}