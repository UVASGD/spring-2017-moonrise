using System;

namespace ItemSpace
{
	public abstract class Potion : ConsumeItem
	{
		public override ItemClass ItemClass {
			get {
				return ItemClass.Potion;
			}
		}
	}
}

