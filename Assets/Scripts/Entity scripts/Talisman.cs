using System;
using System.Collections.Generic;
using Completed;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;

namespace ItemSpace
{
	public class Talisman: AttackItem, SerialOb
	{
		private TalismanType type;
		private TalismanWeight weight;
		private TalismanPrefix prefix;
		private TalismanInfix infix;
		private TalismanSuffix suffix;

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

		private static List<TalismanPrefix> prefixApostrophes = new List<TalismanPrefix>( new[] {
			TalismanPrefix.Soldier, TalismanPrefix.Knight, TalismanPrefix.Captain, 
			TalismanPrefix.Ogre, TalismanPrefix.Titan, TalismanPrefix.Dragon, 
			TalismanPrefix.Medic, TalismanPrefix.Doctor, TalismanPrefix.Surgeon,
			TalismanPrefix.Duke, TalismanPrefix.Lord, TalismanPrefix.King
		});

		private static List<TalismanSuffix> suffixNoThes = new List<TalismanSuffix>( new[] {
			TalismanSuffix.Sight, TalismanSuffix.Strength, TalismanSuffix.Might, TalismanSuffix.Power, TalismanSuffix.Destruction
		});

		public Talisman(TalismanType type, TalismanWeight weight, TalismanPrefix prefix, TalismanInfix infix, TalismanSuffix suffix)
		{
			//if(this.type == null)	
			setup(type,  weight,  prefix,  infix,  suffix);
		}

		public Talisman(){
			
		}

		private void setup(TalismanType type, TalismanWeight weight, TalismanPrefix prefix, TalismanInfix infix, TalismanSuffix suffix){
			this.itemClass = ItemClass.Talisman;
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
			case TalismanPrefix.Great:
				attackMult = 1.15;
				break;
			case TalismanPrefix.Mighty:
				attackMult = 1.25;
				break;
			case TalismanPrefix.Masterful:
				attackMult = 1.4;
				break;
			case TalismanPrefix.Soldier:
				attackMult = 1.2;
				break;
			case TalismanPrefix.Knight:
				attackMult = 1.3;
				break;
			case TalismanPrefix.Captain:
				attackMult = 1.45;
				break;
			case TalismanPrefix.Ogre:
				attackMult = 1.3;
				break;
			case TalismanPrefix.Titan:
				attackMult = 1.4;
				break;
			case TalismanPrefix.Dragon:
				attackMult = 1.6;
				break;
			case TalismanPrefix.Medic:
				hpBonus = 20;
				break;
			case TalismanPrefix.Doctor:
				hpBonus = 30;
				break;
			case TalismanPrefix.Surgeon:
				hpBonus = 40;
				break;
			}

			switch (infix) {
			case TalismanInfix.Bronze:
				attackBonus = 10;
				break;
			case TalismanInfix.Steel:
				attackBonus = 15;
				break;
			case TalismanInfix.Silver:
				attackBonus = 20;
				break;
			case TalismanInfix.Platinum:
				attackBonus = 25;
				break;
			case TalismanInfix.Titanium:
				attackBonus = 30;
				break;
			case TalismanInfix.Diamond:
				attackBonus = 35;
				break;
			case TalismanInfix.Obsidian:
				attackBonus = 40;
				break;
			}

			switch (suffix) {
			case TalismanSuffix.Wind:
				speedMult = 1.15;
				break;
			case TalismanSuffix.Gale:
				speedMult = 1.2;
				break;
			case TalismanSuffix.Storm:
				speedMult = 1.25;
				break;
			}

			name = CreateName (type, weight, prefix, infix, suffix);
		}

		public static string CreateName(TalismanType type, TalismanWeight weight, TalismanPrefix prefix, TalismanInfix infix, TalismanSuffix suffix) {
			string weightStr, prefixStr, infixStr, typeStr, suffixStr;

			if (weight == TalismanWeight.Medium)
				weightStr = "";
			else
				weightStr = weight.ToString () + " ";

			if (prefix == TalismanPrefix.None)
				prefixStr = "";
			else {
				prefixStr = prefix.ToString ();
				if (prefixApostrophes.Contains (prefix))
					prefixStr += "'s ";
				else
					prefixStr += " ";
			}

			if (infix == TalismanInfix.None)
				infixStr = "";
			else
				infixStr = infix.ToString () + " ";

			typeStr = type.ToString ();

			if (suffix == TalismanSuffix.None)
				suffixStr = "";
			else {
				suffixStr = suffix.ToString ();
				if (suffix == TalismanSuffix.Sight)
					suffixStr = "True " + suffixStr;
				if (suffixNoThes.Contains (suffix))
					suffixStr = " of " + suffixStr;
				else
					suffixStr = " of the " + suffixStr;

			}

			return weightStr + prefixStr + infixStr + typeStr + suffixStr;
		}

		public static new Item RandomTalisman() {
			TalismanType type = TalismanType.Talisman;
			TalismanWeight weight = (TalismanWeight)RandomEnum (weightProbs);
			List<int> prefixProbs, suffixProbs;
			if (weight == TalismanWeight.Light) {
				prefixProbs = lightPrefixProbs;
				suffixProbs = lightSuffixProbs;
			} else if (weight == TalismanWeight.Medium) {
				prefixProbs = mediumPrefixProbs;
				suffixProbs = mediumSuffixProbs;
			} else {
				prefixProbs = heavyPrefixProbs;
				suffixProbs = heavySuffixProbs;
			}
			TalismanPrefix prefix = (TalismanPrefix)RandomEnum (prefixProbs);
			TalismanInfix infix = (TalismanInfix)RandomEnum (infixProbs);
			TalismanSuffix suffix = (TalismanSuffix)RandomEnum (suffixProbs);

			return new Talisman (type, weight, prefix, infix, suffix);
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
				if (this.weight == TalismanWeight.Light)
					mult = 0.9;
				else if (this.weight == TalismanWeight.Medium)
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
			setup((TalismanType)Convert.ToDouble(info[0].Value),
				(TalismanWeight)Convert.ToDouble(info[1].Value),
				(TalismanPrefix)Convert.ToDouble(info[2].Value),
				(TalismanInfix)Convert.ToDouble(info[3].Value),
				(TalismanSuffix)Convert.ToDouble(info[4].Value));

			return true;
		}
	}

	public enum TalismanType
	{
		Talisman
	}

	public enum TalismanWeight
	{
		Light,
		Medium,
		Heavy
	}

	public enum TalismanPrefix
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

	public enum TalismanInfix
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

	public enum TalismanSuffix
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

