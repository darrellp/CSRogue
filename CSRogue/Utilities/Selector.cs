using System;
using System.Collections.Generic;

namespace CSRogue.Utilities
{
    public static class Selector<T>
	{
		private static void Swap(IList<T> list, int i1, int i2)
		{
			if (i1 == i2)
			{
				return;
			}
			var temp = list[i1];
			list[i1] = list[i2];
			list[i2] = temp;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Select some elements from and IList. </summary>
		///
		/// <remarks>	
		/// Two important notes.  First is that this function is destructive of list.  The elements of
		/// list may be permuted on return.  Secondly is that if we didn't find n items, we return the
		/// list of what we did find with no error. This means that callers should normally check that
		/// the count in the returned list is equal to their passed in n.  Also notice that if n is the
		/// length of list, then this is essentially a shuffle of the elements of list.  Darrellp,
		/// 10/14/2011. 
		/// </remarks>
		///
		/// <param name="list">			The list to be selected from. </param>
		/// <param name="acceptable">	Criteria to say what's acceptable from the list. </param>
		/// <param name="n">			The count of items we'd like back. </param>
		///
		/// <returns>	A list of found items. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<T> SelectFrom(IList<T> list, Func<T, bool> acceptable = null, int n = 1)
		{
			// Did acceptable criteria default to null?
			if (acceptable == null)
			{
				// Set it to accept everything
				acceptable = v => true;
			}

			// Initialize a list to return
			var ret = new List<T>();
			var available = list.Count;

			// For each selection while any are available
			for (var i = 0; i < n && available > 0; i++)
			{
				// locals
				T selected;

				do
				{
					// Get a random selected object
					var selectedIndex = Rnd.Global.Next(available);
					selected = list[selectedIndex];

					// Swap it out of the available range
					Swap(list, selectedIndex, available - 1);

					// Is it acceptable?
					if (acceptable(selected))
					{
						break;
					}
				}
					// there are no more available
					while (--available > 0);

				// If we didn't run out of available items
				if (available-- > 0)
				{
					// Add the current one to our return list
					ret.Add(selected);
				}
			}

			// Return our list
			return ret;
		}
	}
}
