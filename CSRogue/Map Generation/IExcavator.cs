namespace CSRogue.Map_Generation
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	
	/// Excavator produces the terrain of a map.  It also randomly places items in the map since that
	/// may depend on some knowledge of where rooms are located, etc..  Once instantiated, if an
	/// excavator is used to excavate for a particular map size, it should always produce precisely
	/// the same rooms on the same sized map.  If there is any randomness in the excavation, the
	/// excavator needs to keep a seed around to reproduce the map.  Thus, the excavator is
	/// effectively a very efficient way of storing the map. 
	/// </summary>
	///
	/// <remarks>	Darrellp, 9/21/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public interface IExcavator
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Place all terrain on a map. </summary>
		///
		/// <param name="map">	The map. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		void Excavate(IMap map);
	}
}
