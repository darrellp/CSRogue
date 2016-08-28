using System;
using System.Collections.Generic;
using System.IO;
using CSRogue.Item_Handling;

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
		#endregion

		#region Constructor
		internal FileExcavator(Stream stream)
		{
			StreamReader reader = new StreamReader(stream);

			// While there are lines to read
			while (!reader.EndOfStream)
			{
				// Read a line
				_asciiLines.Add(reader.ReadLine());
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
		public void Excavate(IRoomsMap map)
		{
			// For each line in the file
			for (int iRow = 0; iRow < _asciiLines.Count && iRow < map.Height; iRow++)
			{
				// Read the line
				string currentLine = _asciiLines[iRow];

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
		private static void InsertRow(IMap map, string currentLine, int iRow)
		{
			// For each character in the read line
			for (int iCol = 0; iCol < Math.Min(map.Width, currentLine.Length); iCol++)
			{
				// Produce the data for that character
				TerrainType terrain = TerrainFactory.ProduceTerrain(currentLine[iCol]);
				Item item = ItemInfo.NewItemFromChar(currentLine[iCol]);
				List<Item> items = item == null ? null : new List<Item> { item };
				MapLocationData data = new MapLocationData(terrain, items);

				// and place it in the map
				map[iCol, iRow] = data;
			}
		}
		#endregion
	}
}
