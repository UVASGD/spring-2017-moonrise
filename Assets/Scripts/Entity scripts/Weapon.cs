using System;
using System.Collections.Generic;
using Completed;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;

namespace ItemSpace
{
	public class Weapon : AttackItem, SerialOb
	{
		private WeaponType type;
		private WeaponWeight weight;
		private WeaponPrefix prefix;
		private WeaponInfix infix;
		private WeaponSuffix suffix;

		private int range;

		private int attackBonus, hpBonus;
		private double attackMult, speedMult;

		private static List<int> weightProbs = new List<int> (new[] {
			30, 40, 30
		}),
		lightPrefixProbs = new List<int> (new [] {
			30, 12, 8, 2, 0, 0, 0, 0, 0, 0, 12, 8, 2, 12, 8, 2, 0, 0, 0, 3, 1
		}),
		mediumPrefixProbs = new List<int> (new [] {
			30, 0, 0, 0, 12, 8, 2, 0, 0, 0, 12, 8, 2, 0, 0, 0, 12, 8, 2, 3, 1
		}),
		heavyPrefixProbs = new List<int> (new [] {
			30, 0, 0, 0, 0, 0, 0, 12, 8, 2, 12, 8, 2, 0, 0, 0, 12, 8, 2, 3, 1
		}),
		infixProbs = new List<int> (new [] {
			30, 25, 20, 10, 8, 4, 2, 1
		}),
		lightSuffixProbs = new List<int> (new [] {
			30, 12, 8, 3, 12, 8, 3, 0, 0, 0, 12, 8, 3, 0, 0, 0, 1	
		}),
		mediumSuffixProbs = new List<int> (new [] {
			30, 9, 0, 0, 0, 0, 0, 20, 8, 2, 20, 8, 2, 0, 0, 0, 1	
		}),
		heavySuffixProbs = new List<int> (new [] {
			30, 0, 0, 0, 0, 0, 0, 12, 8, 3, 12, 8, 3, 12, 8, 3, 1	
		});

		private static List<WeaponPrefix> prefixApostrophes = new List<WeaponPrefix>( new[] {
			WeaponPrefix.Soldier, WeaponPrefix.Knight, WeaponPrefix.Captain, 
			WeaponPrefix.Ogre, WeaponPrefix.Titan, WeaponPrefix.Dragon, 
			WeaponPrefix.Medic, WeaponPrefix.Doctor, WeaponPrefix.Surgeon,
			WeaponPrefix.Duke, WeaponPrefix.Lord, WeaponPrefix.King
		});

		private static List<WeaponSuffix> suffixNoThes = new List<WeaponSuffix>( new[] {
			WeaponSuffix.Sight, WeaponSuffix.Strength, WeaponSuffix.Might, WeaponSuffix.Power, WeaponSuffix.Destruction
		});

		public Weapon(WeaponType type, WeaponWeight weight, WeaponPrefix prefix, WeaponInfix infix, WeaponSuffix suffix)
		{
			//if(this.type == null)	
			setup(type,  weight,  prefix,  infix,  suffix);
		}

		public Weapon(){

		}

		private void setup(WeaponType type, WeaponWeight weight, WeaponPrefix prefix, WeaponInfix infix, WeaponSuffix suffix){
			this.itemClass = ItemClass.Weapon;
			this.type = type;
			this.weight = weight;
			this.prefix = prefix;
			this.infix = infix;
			this.suffix = suffix;

			attackBonus = 5;
			attackMult = 1;
			speedMult = 1;
			hpBonus = 0;

			switch (prefix) {
			case WeaponPrefix.Great:
				attackMult = 1.15;
				break;
			case WeaponPrefix.Mighty:
				attackMult = 1.25;
				break;
			case WeaponPrefix.Masterful:
				attackMult = 1.4;
				break;
			case WeaponPrefix.Soldier:
				attackMult = 1.2;
				break;
			case WeaponPrefix.Knight:
				attackMult = 1.3;
				break;
			case WeaponPrefix.Captain:
				attackMult = 1.45;
				break;
			case WeaponPrefix.Ogre:
				attackMult = 1.3;
				break;
			case WeaponPrefix.Titan:
				attackMult = 1.4;
				break;
			case WeaponPrefix.Dragon:
				attackMult = 1.6;
				break;
			case WeaponPrefix.Medic:
				hpBonus = 20;
				break;
			case WeaponPrefix.Doctor:
				hpBonus = 30;
				break;
			case WeaponPrefix.Surgeon:
				hpBonus = 40;
				break;
			}

			switch (infix) {
			case WeaponInfix.Bronze:
				attackBonus = 10;
				break;
			case WeaponInfix.Steel:
				attackBonus = 15;
				break;
			case WeaponInfix.Silver:
				attackBonus = 20;
				break;
			case WeaponInfix.Platinum:
				attackBonus = 25;
				break;
			case WeaponInfix.Titanium:
				attackBonus = 30;
				break;
			case WeaponInfix.Diamond:
				attackBonus = 35;
				break;
			case WeaponInfix.Obsidian:
				attackBonus = 40;
				break;
			}

			switch (suffix) {
			case WeaponSuffix.Wind:
				speedMult = 1.15;
				break;
			case WeaponSuffix.Gale:
				speedMult = 1.2;
				break;
			case WeaponSuffix.Storm:
				speedMult = 1.25;
				break;
			}

			name = CreateName (type, weight, prefix, infix, suffix);
		}

		public static string CreateName(WeaponType type, WeaponWeight weight, WeaponPrefix prefix, WeaponInfix infix, WeaponSuffix suffix) {
			string weightStr, prefixStr, infixStr, typeStr, suffixStr;

			if (weight == WeaponWeight.Medium)
				weightStr = "";
			else
				weightStr = weight.ToString () + " ";

			if (prefix == WeaponPrefix.None)
				prefixStr = "";
			else {
				prefixStr = prefix.ToString ();
				if (prefixApostrophes.Contains (prefix))
					prefixStr += "'s ";
				else
					prefixStr += " ";
			}

			if (infix == WeaponInfix.None)
				infixStr = "";
			else
				infixStr = infix.ToString () + " ";

			typeStr = type.ToString ();

			if (suffix == WeaponSuffix.None)
				suffixStr = "";
			else {
				suffixStr = suffix.ToString ();
				if (suffix == WeaponSuffix.Sight)
					suffixStr = "True " + suffixStr;
				if (suffixNoThes.Contains (suffix))
					suffixStr = " of " + suffixStr;
				else
					suffixStr = " of the " + suffixStr;

			}

			return weightStr + prefixStr + infixStr + typeStr + suffixStr;
		}

		public static new Weapon RandomWeapon() {
			WeaponType type = WeaponType.Crossbow;
			WeaponWeight weight = (WeaponWeight)RandomEnum (weightProbs);
			List<int> prefixProbs, suffixProbs;
			if (weight == WeaponWeight.Light) {
				prefixProbs = lightPrefixProbs;
				suffixProbs = lightSuffixProbs;
			} else if (weight == WeaponWeight.Medium) {
				prefixProbs = mediumPrefixProbs;
				suffixProbs = mediumSuffixProbs;
			} else {
				prefixProbs = heavyPrefixProbs;
				suffixProbs = heavySuffixProbs;
			}
			WeaponPrefix prefix = (WeaponPrefix)RandomEnum (prefixProbs);
			WeaponInfix infix = (WeaponInfix)RandomEnum (infixProbs);
			WeaponSuffix suffix = (WeaponSuffix)RandomEnum (suffixProbs);

			return new Weapon (type, weight, prefix, infix, suffix);
		}

		private static int RandomEnum(List<int> probs) {
			int rand = UnityEngine.Random.Range (0, 100);
			for(int i = 0; i < probs.Count; i++) {
				if (rand < probs [i])
					return i;
				else
					rand -= probs [i];
			}
			return -1; // this point should not be reached
		}

		public int AttackBonus {
			get {
				return attackBonus;
			}
		}

		public double AttackMult {
			get {
				return attackMult;
			}
		}

		public int[] AttackMinMax {
			get {
				double mult;
				if (this.weight == WeaponWeight.Light)
					mult = 0.9;
				else if (this.weight == WeaponWeight.Medium)
					mult = 0.8;
				else 
					mult = 0.7;
				
				return new int[] {(int)(attackMult * attackBonus * mult), (int)(attackMult*attackBonus)};
			}
		}

		public int HpBonus {
			get {
				return hpBonus;
			}
		}

		public double SpeedMult {
			get {
				return speedMult;
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
			setup((WeaponType)Convert.ToDouble(info[0].Value),
				(WeaponWeight)Convert.ToDouble(info[1].Value),
				(WeaponPrefix)Convert.ToDouble(info[2].Value),
				(WeaponInfix)Convert.ToDouble(info[3].Value),
				(WeaponSuffix)Convert.ToDouble(info[4].Value));

			return true;
		}
	}

	// by default, enums have int values and start at 0; 
	// some code in this file relies on this default behavior

	public enum WeaponType
	{
		Crossbow
	}

	public enum WeaponWeight
	{
		Light,
		Medium,
		Heavy
	}

	public enum WeaponPrefix
	{
		None,
		Great,
		Mighty,
		Masterful,
		Soldier,
		Knight,
		Captain,
		Ogre,
		Titan,
		Dragon,
		Medic,
		Doctor,
		Surgeon,
		Fast,
		Quick,
		Lightning,
		Duke,
		Lord,
		King,
		Legendary,
		Ultimate
	}

	public enum WeaponInfix
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

	public enum WeaponSuffix
	{
		None,

		Wind,
		Gale,
		Storm,

		Rogue,
		Assassin,
		Shadow,

		Seer,
		Thief,
		Sniper,

		Eagle,
		Hawk,
		Sight,

		Strength,
		Might,
		Power,
		Destruction
	}
}

