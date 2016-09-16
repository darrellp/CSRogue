using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Interfaces;
using CSRogue.Map_Generation;
using CSRogue.Utilities;
using RogueSC.Map_Objects;
using RogueSC.Utilities;
using SadConsole;
using static RogueSC.Map_Objects.ScRender;

namespace RogueSC
{
    internal class SCMap : GameMap, IRoomsMap
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

        internal CellAppearance GetAppearance(MapCoordinates crd)
        {
            return GetAppearance(crd.Column, crd.Row);
        }

        internal CellAppearance GetAppearance(int iCol, int iRow)
        {
            CellAppearance appearance;

            if (this[iCol, iRow].Items.Count > 0)
            {
                var id = this[iCol, iRow].Items[0].ItemTypeId;
                appearance = ObjectNameToAppearance[Game.Factory.InfoFromId[id].Name];
            }
            else
            {
                appearance = this[iCol, iRow].Appearance;
            }
            return appearance;
        }

        private readonly HashSet<IRoom> _rooms = new HashSet<IRoom>();
        public ISet<IRoom> Rooms => _rooms;
	}
}
