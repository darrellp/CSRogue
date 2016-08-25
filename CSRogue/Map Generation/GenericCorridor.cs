namespace CSRogue.Map_Generation
{
	class GenericCorridor : GenericRoom
	{
		public override bool IsCorridor
		{
			get
			{
				return true;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor from just a width, height and location. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="width">	The width. </param>
		/// <param name="height">	The height. </param>
		/// <param name="location">	The location. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal GenericCorridor(int width, int height, MapCoordinates location) : base(width, height, location) {}
	}
}
