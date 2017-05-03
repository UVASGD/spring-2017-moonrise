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
		private static readonly string[] types = new string[] {
			"Crossbow"	
		}, weights = new string[] {
			"Light", "", "Heavy"
		}, prefixes = new string[] {
			"Great", "Mighty", "Masterful", "Supreme",
			"Fast", "Quick", "Lightning", "Instant",
			"Good", "Great", "Legendary", "Ultimate",
			"Pacifying", "Healing", "Beautiful", "Precious",
			"Dodge", "Duck", "Dip", "Dive",
			"Rock", "Stone", "Marble", "Adamantium",
			""
		}, infixes = new string[] {
			"Obsidian", "Gold", "Diamond", "Chromium",
			"Messenger's", "Jockey's", "Falcon's", "Tempest's",
			"Bronze", "Steel", "Silver", "Platinum",
			"Medic's", "Doctor's", "Surgeon's", "House's",
			"Mist's", "Shadow's", "Air's", "Ether's",
			"Warrior's", "Knight's", "Lord's", "Paladin's",
			""
		}, suffixes = new string[] {
			"of the Sharp Shooter", "of the Sniper", "of the Deadeye", "of the Longshot",
			"of Eurus", "of Notus", "of Zephyrus", "of Boreas",
			"of Piercing", "of Murder", "of Evisceration", "of Annhilation",
			"of Forgetting", "of Reconciliation", "of Peace", "of Harmony",
			"of the Wind", "of the Gale", "of the Storm", "of the Hurricane",
			"of Phalanx", "of Frontem", "of Formate", "of Testudo",
			""
		};

		public Weapon () : this (1, 6, 0, 6, 0, 6, 0) {}

		public Weapon(int weight, int prefixAttr, int prefixTier, int infixAttr, int infixTier, int suffixAttr, int suffixTier)
		{
			setup (weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier);
		}

		public static Weapon RandomWeapon() {
			int weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier;

			weight = UnityEngine.Random.Range (0, weights.Length);

			RandomAttr (out prefixAttr, out prefixTier);
			RandomAttr (out infixAttr, out infixTier);
			RandomAttr (out suffixAttr, out suffixTier);

			return new Weapon(weight, prefixAttr, prefixTier, infixAttr, infixTier, suffixAttr, suffixTier);
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
				return ItemClass.Weapon;
			}
		}
	}
}
