using System;

namespace ItemSpace
{
	public class Armor : EquipItem
	{
		private ArmorWeight weight;
		private ArmorPrefix prefix;
		private ArmorInfix infix;
		private ArmorSuffix suffix;

		private int dodgeBonus, blockBonus;

		public Armor(ArmorType type, ArmorWeight weight, ArmorPrefix prefix, ArmorInfix infix, ArmorSuffix suffix)
		{
			this.itemClass = ItemClass.Armor;

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

