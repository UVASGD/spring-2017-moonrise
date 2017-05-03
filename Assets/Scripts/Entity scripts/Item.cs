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
			int rand = UnityEngine.Random.Range (0, 2); //change this when armor is added
			if (rand == 0) {
				return Weapon.RandomWeapon ();
			} else if (rand == 1) {
				return Talisman.RandomTalisman ();
			}
			else // rand == 2
				return null; //Armor.RandomArmor();
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

