using System;

namespace ItemSpace
{
	public abstract class AttackItem : EquipItem
	{
		protected override void BuffWeight(int weight) {
			base.BuffWeight(weight);
			attackBonus += weight + 2;
		}
	}
}

