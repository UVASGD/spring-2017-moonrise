using System;

namespace ItemSpace
{
	public class Armor : EquipItem
	{
		private ArmorWeight weight;
		private ArmorPrefix prefix;
		private ArmorInfix infix;
		private ArmorSuffix suffix;



		public Armor(ItemClass ic, ArmorWeight weight, ArmorPrefix prefix, ArmorInfix infix, ArmorSuffix suffix)
		{
			itemClass = ic;
			dodgeBonus = 0;
			blockBonus = 0;
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

	public enum ArmorWeight
	{
		Light,
		Heavy
	}

	public enum ArmorPrefix
	{
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
		
	}
}

