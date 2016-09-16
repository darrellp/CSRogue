using System;
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
            var perlin = new PerlinNoise3D();
		    perlin.Frequency = 50.0;
		    perlin.Octaves = 2;
			var excavator = new GridExcavator();
			excavator.Excavate(this, Player);
			foreach (var doorLoc in this.TerrainLocations(new HashSet<TerrainType>() { TerrainType.Door }))
			{
				this[doorLoc].IsDoorOpen = false;
			}
		    for (var iCol = 0; iCol < Width; iCol++)
		    {
		        for (var iRow = 0; iRow < Height; iRow++)
		        {
		            var noise = perlin.ComputePositive(iCol / (3.0 * Width) + 0.333, iRow / (3.0 * Height) + 0.333, 0.5);
		            if (!this.Corridor(iCol, iRow) && this[iCol, iRow].Terrain == TerrainType.Floor && noise < 0.4)
		            {
		                this[iCol, iRow].HasGroundCover = true;
		            }
		        }
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
            Guid id;

            if (this.InView(iCol, iRow) &&
                this[iCol, iRow].Items.Count > 0 &&
                (id = this[iCol, iRow].Items[0].ItemTypeId) != ItemIDs.HeroId)
            {
                appearance = ObjectNameToAppearance[Game.Factory.InfoFromId[id].Name];
            }
            else if (this[iCol, iRow].HasGroundCover)
            {
                appearance = ObjectNameToAppearance["groundCover"];
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
