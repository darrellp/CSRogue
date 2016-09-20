namespace CSRogue.Utilities
{
	public class DieRoll
	{
		internal int DieCount { get; set; }
		internal int DieSides { get; set; }
        internal int Plus { get; set; }

		private void Set( int dieCount, int dieSides, int plus = 0)
		{
			DieCount = dieCount;
			DieSides = dieSides;
		    Plus = plus;
		}

		public DieRoll(int dieCount, int dieSides, int plus = 0)
		{
			Set(dieCount, dieSides, plus);
		}

		public DieRoll(string roll)
		{
			var sides = roll.Split(new [] { 'd', '+' });
			Set(short.Parse(sides[0]), short.Parse(sides[1]), sides.Length > 2 ? short.Parse(sides[2]) : 0);
		}

		public int Roll()
		{
			var sum = 0;

			for (var iDie = 0; iDie < DieCount; iDie++)
			{
				sum += Rnd.Global.Next(1, DieSides + 1);
			}
			return sum + Plus;
		}

		public override string ToString()
		{
			return DieCount + "d" + DieSides + (Plus == 0 ? string.Empty :(" + " + Plus));
		}
	}
}
