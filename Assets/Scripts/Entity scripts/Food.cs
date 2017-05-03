using System;

namespace ItemSpace
{
	public abstract class Food : ConsumeItem
	{
		protected int calories, portions;

		public bool Consumed()
		{
			return portions == 0;
		}

		public int Eat()
		{
			if (portions == 0)
				return 0;
			portions -= 1;
			return calories;
		}

		public override ItemClass ItemClass {
			get {
				return ItemClass.Food;
			}
		}
	}
}

