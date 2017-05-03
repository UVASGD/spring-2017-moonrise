using System;

namespace ItemSpace
{
	public abstract class AttackItem : EquipItem
	{
		public int[] AttackMinMax {
			get {
				double mult;
				if (this.weight == 0)
					mult = 0.9;
				else if (this.weight == 1)
					mult = 0.8;
				else 
					mult = 0.7;

				return new int[] {(int)(attackMult * attackBonus * mult), (int)(attackMult*attackBonus)};
			}
		}
	}
}

