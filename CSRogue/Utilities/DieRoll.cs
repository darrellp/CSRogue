using System;

namespace CSRogue.Utilities
{
	public class DieRoll
	{
		internal int DieCount { get; set; }
		internal int DieSides { get; set; }

		private void Set( int dieCount, int dieSides)
		{
			DieCount = dieCount;
			DieSides = dieSides;
		}

		internal DieRoll(int dieCount, int dieSides)
		{
			Set(dieCount, dieSides);
		}

		internal DieRoll(string roll)
		{
			String[] sides = roll.Split(new[] {'d'});
			Set(Int16.Parse(sides[0]), Int16.Parse(sides[1]));
		}

		internal int Roll()
		{
			int sum = 0;

			for (int iDie = 0; iDie < DieCount; iDie++)
			{
				sum += Rnd.Global.Next(1, DieSides + 1);
			}
			return sum;
		}

		public override string ToString()
		{
			return DieCount + "d" + DieSides;
		}
	}
}
