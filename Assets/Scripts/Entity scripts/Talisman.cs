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
		private static readonly string[] types = new string[] {
			"Talisman"	
		}, weights = new string[] {
			"Light", "", "Heavy"
		}, prefixes = new string[] {
			"Bewitched", "Sacrilegious", "Murderous", "Splendid",
			"Energetic", "Striking", "Bombastic", "Enchanted",
			"Chilling", "Fiery", "Shocking", "Toxic",
			"Restoring", "Healing", "Rejuvenating", "Holy",
			"Shimmering", "Glimmering", "Flashing", "Mystical",
			"Radiant", "Mysterious", "Sacred", "Ancient",
			""
		}, infixes = new string[] {
			"Murderer's", "Captain's", "Mystic's", "Shaman's",
			"Thief's", "Athlete's", "Sprinter's", "Clairvoyant's",
			"Hunter's", "Soldier's", "Barbarian's", "Champion's",
			"Child's", "Lover's", "Doctor's", "Priest's",
			"Coward's", "Bandit's", "Herald's", "Spy's",
			"Defender's", "Poet's", "Mother's", "Ancient's",
			""
		}, suffixes = new string[] {
			"of Destruction", "of Maiming", "of the Inferno", "of Hell",
			"of Far Sight", "of the Falcon", "of the Cheetah", "of Lightning",
			"of Bruising", "of Anger", "of Murder", "of Power",
			"of the Fox", "of the Wolf", "of the Turtle", "of Light",
			"of the Lizard", "of the Shark", "of the Gorilla", "of Lamentation",
			"of the Panther", "of the Cockroach", "of the Beetle", "of the Rhino",
			""
		};

		public Talisman () : this (1, 6, 0, 6, 0, 6, 0) {}

		public Talisman(int weight, int prefixAttr, int prefixTier, int infixAttr, int infixTier, int suffixAttr, int suffixTier)
		{
			setup (weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier);
			setType ("talisman");
		}

		public static Talisman RandomTalisman() {
			int weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier;

			weight = UnityEngine.Random.Range (0, weights.Length);

			RandomAttr (out prefixAttr, out prefixTier);
			RandomAttr (out infixAttr, out infixTier);
			RandomAttr (out suffixAttr, out suffixTier);

			return new Talisman(weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier);
		}

		protected override string[] Types {
			get {
				return types;
			}
		}
	
		protected override string[] Weights {
			get {
				return weights;
			}
		}

		protected override string[] Prefixes {
			get {
				return prefixes;
			}
		}

		protected override string[] Infixes {
			get {
				return infixes;
			}
		}

		protected override string[] Suffixes {
			get {
				return suffixes;
			}
		}

		public override ItemClass ItemClass {
			get {
				return ItemClass.Talisman;
			}
		}
	}
}
