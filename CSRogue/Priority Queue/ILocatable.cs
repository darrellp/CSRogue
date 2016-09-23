using System;

namespace Priority_Queue
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	
	/// In a priority queue which supports deletion, elements must keep track of their position
	/// within the queue's heap list.  This interface supports that. 
	/// </summary>
	///
	/// <remarks>	Darrellp, 2/17/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface ILocatable : IComparable
	{
		///<summary>
		/// Returns the index stored in SetIndex
		///</summary>
		int Index { get; set; }
	}
}
