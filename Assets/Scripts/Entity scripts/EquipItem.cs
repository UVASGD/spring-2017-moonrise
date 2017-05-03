using System;

namespace ItemSpace
{
	public abstract class EquipItem : Item
	{
		protected int attackBonus, hpBonus;
		protected double attackMult, speedMult;
		protected int dodgeBonus, blockBonus;

		public int AttackBonus {
			get {
				return this.attackBonus;
			}
		}

		public int HpBonus {
			get {
				return this.hpBonus;
			}
		}

		public double AttackMult {
			get {
				return this.attackMult;
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

