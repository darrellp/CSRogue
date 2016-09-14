using System;

namespace CSRogue.Utilities
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Random number generator. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class Rnd
	{
	    private static Random _global = new Random();
	    private static bool _validToSetSeed = true;

	    internal static Random Global => _global;

	    public static void SetGlobalSeed(int iSeed = -1)
	    {
	        if (!_validToSetSeed)
	        {
	            throw new RogueException("Setting seed in an invalid context - either already set or random numbers already taken");
	        }
	        _validToSetSeed = false;
	        if (iSeed != -1)
	        {
	            _global = new Random(iSeed);
	        }
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
        public static int GlobalNext(int exclusiveMax)
        {
            _validToSetSeed = false;
            return Global.Next(exclusiveMax);
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
        public static int GlobalNext(int inclusiveMin, int exclusiveMax)
		{
			if (inclusiveMin > exclusiveMax)
			{
				throw new ArgumentException("Min < Max in Next");
			}
			return Global.Next(inclusiveMin, exclusiveMax);
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
		internal static void GlobalSpan(int start, int end, int minWidth, out int spanStart, out int spanEnd)
		{
			if (minWidth > end - start)
			{
				throw new RogueException("Width isn't wide enough to make span in RandomSpan");
			}
			var val1 = GlobalNext(start, end + 1);
			var val2 = GlobalNext(start, end + 1);

			while (Math.Abs(val1 - val2) < minWidth)
			{
				val1 = GlobalNext(start, end + 1);
				val2 = GlobalNext(start, end + 1);
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
