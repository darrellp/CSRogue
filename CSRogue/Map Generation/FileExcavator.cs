﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSRogue.Interfaces;
using CSRogue.Item_Handling;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	
	/// Excavate rooms in a map by reading an ascii version on a stream. The ascii version of maps
	/// doesn't allow for all nuances of a standard map (i.e., vertical vs horizontal doors) so this
	/// is not suggested practice in general - mainly for testing purposes. 
	/// </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class FileExcavator : IExcavator
	{
		#region Private variables
		private readonly List<string> _asciiLines = new List<string>();
	    private IItemFactory _factory;
        private readonly Dictionary<char, Guid> _charToId = new Dictionary<char, Guid>();
		private Func<IMapLocationData> _dataCreator;
		#endregion

		#region Constructor

		public FileExcavator(
			string mapString, 
			IItemFactory factory,
			Func<IMapLocationData> dataCreator = null) : this(new StringReader(mapString), factory, dataCreator){ }

		public FileExcavator(TextReader reader, IItemFactory factory, Func<IMapLocationData> dataCreator = null)
		{
			_dataCreator = dataCreator ?? (() => new MapLocationData());
			var line = reader.ReadLine();
            while (line != null)
            {
                _asciiLines.Add(line);
                line = reader.ReadLine();
            }
            _factory = factory;
            if (_factory != null)
            {
                foreach (var itemInfo in factory.InfoFromId)
                {
                    _charToId[itemInfo.Value.Character] = itemInfo.Key;
                }
            }
        }
        #endregion

        #region Excavate by reading from a file
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Excavates the rooms for a level based on a text stream. </summary>
        ///
        /// <remarks>	Darrellp, 9/16/2011. </remarks>
        ///
        /// <param name="map">		The map to be excavated. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Excavate(IMap map)
        {
            map.Width = _asciiLines.Select(l => l.Length).Aggregate((l1, l2) => Math.Max(l1, l2));
            map.Height = _asciiLines.Count;

            // For each line in the file
            for (var iRow = 0; iRow < _asciiLines.Count && iRow < map.Height; iRow++)
			{
				// Read the line
				var currentLine = _asciiLines[iRow];

				// Insert the line into the current row
				InsertRow(map, currentLine, iRow);
			}
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Inserts a single read row into the map. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="map">			The map to modify. </param>
		/// <param name="currentLine">	The current line read from the stream. </param>
		/// <param name="iRow">			The row to modify. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void InsertRow(IMap map, string currentLine, int iRow)
		{
			// For each character in the read line
			for (var iCol = 0; iCol < Math.Min(map.Width, currentLine.Length); iCol++)
			{
                // TODO: Once we have varied walkable terrains, how do we get creatures and textures in the same spot?
                // The answer might very well be "we don't".  This is a convenience function for use in special situations
                // If you want something other than this default, place the monsters afterwards by hand.

				// Produce the data for that character
				var terrain = TerrainFactory.ProduceTerrain(currentLine[iCol]);

				var item = _charToId.ContainsKey(currentLine[iCol])
					? _factory.InfoFromId[_charToId[currentLine[iCol]]].CreateItem(null) : null;
                
				var items = item == null ? null : new List<IItem> { item };
				var data = _dataCreator();
				data.Terrain = terrain;
				data.Items = items;

				// and place it in the map
				map[iCol, iRow] = data;
				if (terrain != TerrainType.Floor &&
				    terrain != TerrainType.Door &&
				    terrain != TerrainType.StairsDown &&
				    terrain != TerrainType.StairsUp)
				{
					map.SetBlocksView(iCol, iRow);
				}
				else
				{
					map.SetBlocksView(iCol, iRow, false);
                    map.SetWalkable(iCol, iRow);
				}
			}
		}
		#endregion
	}
}
