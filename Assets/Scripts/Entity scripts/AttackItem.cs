using System;

namespace ItemSpace
{
	public abstract class AttackItem : EquipItem
	{
		public int[] AttackMinMax {
			get {
				int low = 2 + weight + attackBonus;
				int high = low + 3;

				return new int[] {(int)(low * attackMult), (int)(high * attackMult)};
			}
		}
	}
}

