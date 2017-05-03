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
		private string prefix;
		private string infix;
		private string suffix;

		//private int range;


		private double [,] bonuses = new double[,]
		
		{
			{3,		6,		12,		25	}, //0: attack bonus
			{1.1,	1.25,	1.5,	2.0	}, //1: attack multiplier
			{1.1,	1.25,	1.5,	2.0	}, //2: speed multiplier
			{2,		5,		10,		15	}, //3: hp bonus
			{1.1,	1.25,	1.5,	2.0	}, //4: dodge
			{1.1,	1.25,	1.5,	2.0	}  //5: block
		};

		private string[,,] labels = new string[,,] {
			{//prefix
				{"Chilling", 	"Fiery", 		"Shocking", 	"Toxic"},
				{"Bewitched", 	"Sacrilegious", "Murderous", 	"Splendid"},
				{"Shimmering", 	"Glimmering", 	"Flashing", 	"Radiant"},
				{"Mysterious", 	"Mystical", 	"Sacred", 		"Ancient"},
				{"Restoring", 	"Healing", 		"Rejuvenating", "Holy"},
				{"Energetic", 	"Striking", 	"Bombastic", 	"Enchanted"}
			},
			{//infix
				{"Hunter\'s",  "Soldier\'s",  "Barbarian\'s",  "Champion\'s"},
				{"Murderer\'s",  "Captain\'s",  "Mystic\'s",  "Shaman\'s"},
				{"Coward\'s",  "Bandit\'s",  "Herald\'s",  "Spy\'s"},
				{"Defender\'s",  "Poet\'s",  "Mother\'s",  "Ancient\'s"},
				{"Child\'s",  "Lover\'s",  "Doctor\'s",  "Priest\'s"},
				{"Thief\'s",  "Athlete\'s",  "Sprinter\'s",  "Clairvoyant\'s"}
			},
			{//suffix
				{"of Bruising",  "of Anger",  "of Murder",  "of Power"},
				{"of Destruction",  "of Maiming",  "of the Inferno",  "of Hell"},
				{"of the Lizard",  "of the Shark",  "of the Gorilla",  "of Lamentation"},
				{"of the Panther",  "of the Cockroach",  "of the Beetle",  "of the Rhino"},
				{"of the Fox",  "of the Wolf",  "of the Turtle",  "of Light"},
				{"of Far-Sight",  "of the Falcon",  "of the Cheetah",  "of Lightning"}
			}
			};



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

		public Talisman(TalismanWeight weight, int[] prefix, int[] infix, int[] suffix)
		{
			this.itemClass = ItemClass.Talisman;
			this.type = TalismanType.Talisman;

			this.weight = weight;
			this.prefix = labels[1, prefix[0], prefix[1]];
			this.infix = labels[2, infix[0], infix[1]];
			this.suffix = labels[3, suffix[0], suffix[1]];

			attackBonus = 0;
			attackMult = 1;
			speedMult = 1;
			hpBonus = 0;
			dodgeBonus = 0;
			blockBonus = 0;


			int[][] fixes = { prefix, infix, suffix };

			foreach (int [] fix in fixes) {
				if (fix [0] == 0)
					attackBonus += (int)bonuses [0, fix [1]];
				else if (fix [0] == 1)
					attackMult *= bonuses [1, fix [1]];
				else if (fix [0] == 2)
					speedMult *= bonuses [2, fix [1]];
				else if (fix [0] == 3)
					hpBonus += (int)bonuses [3, fix [1]];
				else if (fix [0] == 4)
					dodgeBonus += (int)bonuses [4, fix [1]];
				else if (fix [0] == 5)
					blockBonus += (int)bonuses [5, fix [1]];
			}

			name = this.prefix + " " + this.infix + " Talisman " + this.suffix;
				
				
			//setup(type,  weight,  prefix,  infix,  suffix);
		}

		public Talisman(){
			
		}

		private void setup(TalismanType type, TalismanWeight weight, TalismanPrefix prefix, TalismanInfix infix, TalismanSuffix suffix){
			this.itemClass = ItemClass.Talisman;
			this.type = type;
			this.weight = weight;
			this.prefix = prefix.ToString();
			this.infix = infix.ToString();
			this.suffix = suffix.ToString();

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

		public static Item RandomTalisman() {
			TalismanWeight weight = (TalismanWeight)RandomEnum (weightProbs);

			int currentLevel = GameManager.instance.level;
			double fractionTimeLeft = GameManager.instance.timeLeft / GameManager.instance.initialTime;

			double determiner = Math.Floor (currentLevel + (1 / fractionTimeLeft) * 2);
			int tier = 0;
			int[] prefix, infix, suffix;

			if (determiner <= 5)
				tier = 0;
			else if (determiner <= 12)
				tier = 1;
			else if (determiner <= 19)
				tier = 2;
			else if (determiner > 19)
				tier = 3;

			prefix = new int[] { UnityEngine.Random.Range (0, 4), tier };
			infix = new int[] { UnityEngine.Random.Range (0, 4), tier };
			suffix = new int[] { UnityEngine.Random.Range (0, 4), tier };


			return new Talisman (weight, prefix, infix, suffix);
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
			

		virtual public XElement serialize(){
			XElement node = new XElement("weapon",
				new XElement("type", (int)this.type),
				new XElement("weight", weight),
				new XElement("prefix", prefix),
				new XElement("infix", infix),
				new XElement("suffix", suffix));

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

