using System;
using System.Collections.Generic;

namespace ItemSpace
{
	public class EquippedItemSet
	{
		/* An item can only be equipped if the no item of the same type is currently equipped.
		 */

		private Weapon weapon;
		private Armor armor;
		private Talisman talisman;

		public EquippedItemSet()
		{
			
		}

		/// <summary>
		/// Equip the specified item.
		/// Fails if there is already an equipped item of the same item class, 
		/// or if the item class is not included in this instance.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Whether the item was successfully equipped.</returns>
		public bool Equip(Item item)
		{
			if(item is Weapon) {
				if(weapon == null) {
					weapon = (Weapon)item;
					return true;
				}
			} else if (item is Armor) {
				if(armor == null) {
					armor = (Armor)item;
					return true;
				}
			} else if (item is Talisman) {
				if(talisman == null) {
					talisman = (Talisman)item;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Unequip the item of the specified item class.
		/// </summary>
		/// <param name="ic">Item class.</param>
		/// <returns>The unequipped item.</returns>
		public Item Unequip(ItemClass ic)
		{
			Item val = null;
			if(ic == ItemClass.Weapon) {
				val = weapon;
				weapon = null;
			} else if (ic == ItemClass.Armor) {
				val = armor;
				armor = null;
			} else if (ic == ItemClass.Talisman) {
		val = talisman;
		talisman = null;
			}
			return val;
		}

		public Weapon Weapon {
			get {
				return weapon;
			}
		}

		public Armor Armor {
			get {
				return armor;
			}
		}

		public Talisman Talisman {
			get {
				return this.talisman;
			}
		}
	}
}

