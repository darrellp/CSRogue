using System;

namespace CSRogue.Utilities
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Random number generator. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class Rnd
	{
		private Random _rnd;

		internal static Rnd Global => PerThreadSingleton<Rnd>.Instance();

	    internal Rnd(int seed)
		{
			SetSeed(seed);
		}

		public Rnd() : this(-1) {}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Mirrors Random.Next(). </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <returns>	Random non-negative integer. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Next()
		{
			return _rnd.Next();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Mirrors Random.Next(). </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="exclusiveMax">	The exclusive maximum. </param>
		///
		/// <returns>	Random non-negative integer less than exclusiveMax. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Next(int exclusiveMax)
		{
			return _rnd.Next(exclusiveMax);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Mirrors Random.Next(). </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="inclusiveMin">	The inclusive minimum. </param>
		/// <param name="exclusiveMax">	The exclusive maximum. </param>
		///
		/// <returns>	
		/// Random non-negative integer less than exclusiveMax and greater or equal to inclusiveMin. 
		/// </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Next(int inclusiveMin, int exclusiveMax)
		{
			if (inclusiveMin > exclusiveMax)
			{
				throw new ArgumentException("Min < Max in Next");
			}
			return _rnd.Next(inclusiveMin, exclusiveMax);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Sets the seed for the random number generator. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="seed">	The seed. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void SetSeed(int seed)
		{
			_rnd = seed == -1 ? new Random() : new Random(seed);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Sets the seed to a random value. </summary>
		///
		/// <remarks>	Darrellp, 9/21/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void SetSeed()
		{
			SetSeed(-1);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Create a random interval of length at least minWidth between start and end. </summary>
		///
		/// <remarks>	
		/// The range for the interval is start through end inclusive.  The difference between the return
		/// values will be at least minWidth, again inclusive. Darrellp, 9/18/2011. 
		/// </remarks>
		///
		/// <param name="start">		The start value of the range. </param>
		/// <param name="end">			The end value of the range. </param>
		/// <param name="minWidth">		Minimum width of the returned interval. </param>
		/// <param name="spanStart">	[out] The span start. </param>
		/// <param name="spanEnd">		[out] The span end. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void RandomSpan(int start, int end, int minWidth, out int spanStart, out int spanEnd)
		{
			if (minWidth > end - start)
			{
				throw new RogueException("Width isn't wide enough to make span in RandomSpan");
			}
			int val1 = _rnd.Next(start, end + 1);
			int val2 = _rnd.Next(start, end + 1);

			while (Math.Abs(val1 - val2) < minWidth)
			{
				val1 = _rnd.Next(start, end + 1);
				val2 = _rnd.Next(start, end + 1);
			}
			if (val1 < val2)
			{
				spanStart = val1;
				spanEnd = val2;
			}
			else
			{
				spanStart = val2;
				spanEnd = val1;
			}
		}
	}
}
