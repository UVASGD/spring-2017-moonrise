using System;

namespace ItemSpace
{
	public abstract class EquipItem : Item
	{
		protected int hpBonus;
		protected double speedMult;
		protected int dodgeBonus, blockBonus;

		public int HpBonus {
			get {
				return this.hpBonus;
			}
		}

		public double SpeedMult {
			get {
				return this.speedMult;
			}
		}

		public int DodgeBonus {
			get {
				return this.dodgeBonus;
			}
		}

		public int BlockBonus {
			get {
				return this.blockBonus;
			}
		}
	}
}

