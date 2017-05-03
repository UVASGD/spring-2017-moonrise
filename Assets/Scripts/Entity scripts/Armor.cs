using System;
using System.Collections.Generic;
using Completed;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;

namespace ItemSpace
{
	public class Armor : EquipItem, SerialOb
	{
		private ArmorType type;
		private ArmorWeight weight;
		private ArmorPrefix prefix;
		private ArmorInfix infix;
		private ArmorSuffix suffix;

		private int dodgeBonus, blockBonus;

		public Armor(ArmorType type, ArmorWeight weight, ArmorPrefix prefix, ArmorInfix infix, ArmorSuffix suffix)
		{
			setup (type, weight, prefix, infix, suffix);
		}

		private void setup(ArmorType type, ArmorWeight weight, ArmorPrefix prefix, ArmorInfix infix, ArmorSuffix suffix)
		{
			this.itemClass = ItemClass.Armor;
			this.type = type;
			this.weight = weight;
			this.prefix = prefix;
			this.infix = infix;
			this.suffix = suffix;

			dodgeBonus = 0;
			blockBonus = 0;

			name = CreateName (type, weight, prefix, infix, suffix);
		}

		public static Armor RandomArmor() {
			ArmorType type = ArmorType.Armor;
			ArmorWeight weight = ArmorWeight.Light;
			ArmorPrefix prefix = ArmorPrefix.Soldier;
			ArmorInfix infix = ArmorInfix.Bronze;
			ArmorSuffix suffix = ArmorSuffix.None;

			return new Armor(type, weight, prefix, infix, suffix);
		}

		public static string CreateName(ArmorType type, ArmorWeight weight, ArmorPrefix prefix, ArmorInfix infix, ArmorSuffix suffix) {
			string weightStr, prefixStr, infixStr, typeStr, suffixStr;

			weightStr = weight.ToString () + " ";

			if (prefix == ArmorPrefix.None)
				prefixStr = "";
			else {
				prefixStr = prefix.ToString ();
				prefixStr += " ";
			}

			if (infix == ArmorInfix.None)
				infixStr = "";
			else
				infixStr = infix.ToString () + " ";

			typeStr = type.ToString ();

			if (suffix == ArmorSuffix.None)
				suffixStr = "";
			else {
				suffixStr = suffix.ToString ();
				suffixStr = " of the " + suffixStr;

			}

			return weightStr + prefixStr + infixStr + typeStr + suffixStr;
		}

		public int DodgeBonus {
			get {
				return dodgeBonus;
			}
		}

		public int BlockBonus {
			get {
				return blockBonus;
			}
		}

		virtual public XElement serialize(){
			XElement node = new XElement("weapon",
				new XElement("type", (int)this.type),
				new XElement("weight", (int)this.weight),
				new XElement("prefix", (int)this.prefix),
				new XElement("infix", (int)this.infix),
				new XElement("suffix", (int)this.suffix));

			return node;
		}

		virtual public bool deserialize(XElement s){
			List<XElement> info = s.Descendants().ToList();
			setup((ArmorType)Convert.ToDouble(info[0].Value),
				(ArmorWeight)Convert.ToDouble(info[1].Value),
				(ArmorPrefix)Convert.ToDouble(info[2].Value),
				(ArmorInfix)Convert.ToDouble(info[3].Value),
				(ArmorSuffix)Convert.ToDouble(info[4].Value));

			return true;
		}
	}

	public enum ArmorType
	{
		Armor
	}

	public enum ArmorWeight
	{
		Light,
		Heavy
	}

	public enum ArmorPrefix
	{
		None,
		Bowman,
		Archer,
		Sniper,
		Soldier,
		Warrior,
		Knight,
		Thief,
		Rogue,
		Assassin,
		Living,
		Immortal,
		King,
		Emperor
	}

	public enum ArmorInfix
	{
		None,
		Bronze,
		Steel,
		Silver,
		Platinum,
		Titanium,
		Diamond,
		Obsidian
	}

	public enum ArmorSuffix
	{
		None
	}
}

