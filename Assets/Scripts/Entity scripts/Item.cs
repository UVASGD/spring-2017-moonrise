using UnityEngine;
using System;
using Completed;
using System.Xml.Linq;

namespace ItemSpace
{
	public abstract class Item : SerialOb
	{
		protected string name, description;
		protected ItemClass itemClass;

		public static Item RandomItem() {
			int rand = UnityEngine.Random.Range (0, 3);
			Debug.Log ("Random Item");
			if (rand == 0) {
				return Weapon.RandomWeapon ();
			} else if (rand == 1) {
				Debug.Log ("Random tal");
				return Talisman.RandomTalisman ();
			} else {
				Debug.Log ("Random armor");
				return Armor.RandomArmor ();
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public string Description {
			get {
				return description;
			}
		}

		public ItemClass ItemClass {
			get {
				return itemClass;
			}
		}

		virtual public XElement serialize(){
			return new XElement("Item");
		}

		virtual public bool deserialize(XElement s){
			return false;
		}

		// void PickUp();
	}
}

