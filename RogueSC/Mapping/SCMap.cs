using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;
using RogueSC.Map_Objects;
using RogueSC.Utilities;

namespace RogueSC
{
    internal class SCMap : GameMap
	{
		public SCMap(int height, int width, int fovRadius, Game game, IItemFactory factory) :
			base(
				height, width,
				fovRadius,
				game,
				(IPlayer)factory.InfoFromId[ItemIDs.HeroId].CreateItem(null),
				() => new SCMapLocationData())
		{
			var excavator = new GridExcavator();
			excavator.Excavate(this, Player);
			foreach (var doorLoc in this.TerrainLocations(new HashSet<TerrainType>() { TerrainType.Door }))
			{
				this[doorLoc].IsDoorOpen = false;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	
		/// Indexer to get item collections from a spot on the map.  Indexing order is column then row!! 
		/// </summary>
		///
		/// <value>	The indexed item collection. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public new SCMapLocationData this[int iCol, int iRow] => (SCMapLocationData)base[iCol, iRow];

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	
		/// Indexer to get item collections from a spot on the map using MapCoordinates 
		/// </summary>
		///
		/// <value>	The indexed item collection. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public new SCMapLocationData this[MapCoordinates loc] => (SCMapLocationData)base[loc];
	}
}
