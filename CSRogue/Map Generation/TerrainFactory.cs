using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
	#region Terrain types
	public enum TerrainType
	{
		OffMap,
		HorizontalWall,
		VerticalWall,
		Corner,
		Floor,
		Door,
		StairsUp,
		StairsDown,
		Wall
	} 
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Produce terrain from parameters. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class TerrainFactory
	{
		#region Mapping table
		static private readonly Dictionary<char, TerrainType> MapCharToTerrain = new Dictionary<char, TerrainType>
		    {
		        {'.', TerrainType.Floor},
				{'#', TerrainType.Wall},
		        {'-', TerrainType.HorizontalWall},
		        {'|', TerrainType.VerticalWall},
		        {'+', TerrainType.Corner},
				{'~', TerrainType.Door},
		        {' ', TerrainType.OffMap},
				{'>', TerrainType.StairsDown},
				{'<', TerrainType.StairsUp}
		    };

		static private readonly Dictionary<TerrainType, char> MapTerrainToChar = new Dictionary<TerrainType, char>();
		#endregion

		#region Static Constructor
		static TerrainFactory()
		{
			foreach (var character in MapCharToTerrain.Keys)
			{
				MapTerrainToChar[MapCharToTerrain[character]] = character;
			}
		}
		#endregion

		#region Terrain production
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Produce terrain from the corresponding character. </summary>
		///
		/// <remarks>	
		/// Unknown characters are converted to floor so that items on top of the floor get the right
		/// terrain.  Darrellp, 9/16/2011. 
		/// </remarks>
		///
		/// <param name="ch">	The character to convert to terrain. </param>
		///
		/// <returns>	. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static TerrainType ProduceTerrain(char ch)
		{
			return MapCharToTerrain.ContainsKey(ch) ? MapCharToTerrain[ch] : TerrainType.Floor;
		} 
		#endregion

		#region Queries
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Inverse of ProduceTerrain - maps terrains to characters. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="terrain">	The terrain to be mapped. </param>
		///
		/// <returns>	The character corresponding to the terrain. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static char TerrainToChar(TerrainType terrain)
		{
			return MapTerrainToChar[terrain];
		}
		#endregion
	}
}
