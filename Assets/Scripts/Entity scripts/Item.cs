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
			return Weapon.RandomItem ();
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

