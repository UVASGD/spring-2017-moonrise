using UnityEngine;
using System;
using Completed;
using System.Xml.Linq;

namespace ItemSpace
{
	public abstract class Item : SerialOb
	{
		protected string name, description;

		public static Item RandomItem() {
			int rand = UnityEngine.Random.Range (0, 3);
			if (rand == 0) {
				Debug.Log ("Random weapon");
				return Weapon.RandomWeapon ();
			} else if (rand == 1) {
				Debug.Log ("Random talisman");
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

		public abstract ItemClass ItemClass {
			get;
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

