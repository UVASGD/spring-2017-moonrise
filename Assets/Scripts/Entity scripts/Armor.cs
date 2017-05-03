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
		
		private static readonly string[] types = new string[] {
			"Armor"	
		}, weights = new string[] {
			"Light", "", "Heavy"
		}, prefixes = new string[] {
			"Yelling", "Shouting", "Screaming", "Bellowing",
			"Lucky", "Fortunate", "Incredible", "Legendary",
			"Strengthening", "Aggressive", "Empowering", "Herculean",
			"Refreshing", "Invigorating", "Restoring", "Undying",
			"Dodgy", "Evasive", "Side-stepping", "Untouchable",
			"Enveloping", "Protecting", "Shielding", "Impenetrable",
			""
		}, infixes = new string[] {
			"Prince's", "King's", "Emperor's", "Deity's",
			"Thief's", "Rogue's", "Assassin's", "Shadow's",
			"Peasant's", "Fighter's", "Gladiator's", "Predator's",
			"Student's", "Scholar's", "Associate's", "Professor's",
			"Bowman's", "Archer's", "Sniper's", "Deadeye's",
			"Guard's", "Defender's", "Veteran's", "Savior's",
			""
		}, suffixes = new string[] {
			"of Fierceness", "of Viciousness", "of Savagery", "of Ruthlessness",
			"of Swiftness", "of Speed", "of Velocity", "of Haste",
			"of Strength", "of the Fighter", "of the Champion", "of the Giant",
			"of Hardiness", "of Survival", "of Life", "of Immortality",
			"of the Hunter", "of the Lynx", "of the Tempest", "of Shadows",
			"of Iron", "of Bronze", "of Steel", "of Mithril",
			""
		};
		public Armor () : this (1, 6, 0, 6, 0, 6, 0) {}

		public Armor(int weight, int prefixAttr, int prefixTier, int infixAttr, int infixTier, int suffixAttr, int suffixTier)
		{
			setup (weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier);
			setType ("armor");
		}

		protected override void BuffWeight(int weight) {
			base.BuffWeight (weight);
			if (weight == 0) {
				dodgeBonus *= 1.05;
			} else if (weight == 1) {
				dodgeBonus *= 1.025;
				fortifyBonus *= 1.025;
			} else {
				fortifyBonus *= 1.05;
			}
		}

		public static Armor RandomArmor() {
			int weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier;

			weight = UnityEngine.Random.Range (0, weights.Length);

			RandomAttr (out prefixAttr, out prefixTier);
			RandomAttr (out infixAttr, out infixTier);
			RandomAttr (out suffixAttr, out suffixTier);

			return new Armor(weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier);
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
				return ItemClass.Armor;
			}
		}
	}
}

