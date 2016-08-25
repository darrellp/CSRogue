using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSRogue.RogueEventArgs;
using CSRogue.GameControl;
using CSRogue.Item_Handling;
using CSRogue.Items;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Map for CS Rogue. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class Map
	{
		#region Private variables
		private readonly MapLocationData[][] _map;
		private FOV _fov;
		private readonly Game _game;
		#endregion

		#region Properties
		public Player Player { get; }
		public int Width { get; }
		public int Height { get; }
		public MapCoordinates HeroPosition => Player.Location;

	    public HashSet<GenericRoom> Rooms { get; private set; }
		public FOV FOV
		{
			get
			{
				return _fov;
			}
			set
			{
				_fov = value;
			}
		}
		#endregion

		#region Constructor
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Default constructor. </summary>
		///
		/// <remarks>	Darrellp, 9/15/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Map(Game game = null) : this(150, 100, game)
		{
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor with width and height. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="width">	The width. </param>
		/// <param name="height">	The height. </param>
		/// <param name="game">		The game we're involved in. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Map(int width, int height, Game game = null)
		{
			Player = new Player();
			Width = width;
			Height = height;
			Rooms = new HashSet<GenericRoom>();
			_game = game;
			_fov = null;

			_map = new MapLocationData[Width][];
			for (int iCol = 0; iCol < Width; iCol++)
			{
				_map[iCol] = new MapLocationData[Height];
				for (int iRow = 0; iRow < Height; iRow++)
				{
					_map[iCol][iRow] = new MapLocationData();
				}
			}
		}
		#endregion

		#region Indexer
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	
		/// Indexer to get item collections from a spot on the map.  Indexing order is column then row!! 
		/// </summary>
		///
		/// <value>	The indexed item collection. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public MapLocationData this[int iCol, int iRow]
		{
			get
			{
				return _map[iCol][iRow];
			}
			set
			{
				_map[iCol][iRow] = value;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	
		/// Indexer to get item collections from a spot on the map using MapCoordinates 
		/// </summary>
		///
		/// <value>	The indexed item collection. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public MapLocationData this[MapCoordinates location]
		{
			get
			{
				return _map[location.Column][location.Row];
			}
			set
			{
				_map[location.Column][location.Row] = value;
			}
		} 

		#endregion

		#region Modification
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Checks the terrain to see if the creature can move by offset. </summary>
		///
		/// <remarks>	Darrellp, 10/15/2011. </remarks>
		///
		/// <param name="location">	The location to check. </param>
		///
		/// <returns>	true if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal bool ValidTerrainForMove(MapCoordinates location)
		{
			return this[location].Terrain != TerrainType.Wall;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Query if there is a creature at a location. </summary>
		///
		/// <remarks>	Darrellp, 10/15/2011. </remarks>
		///
		/// <param name="location">	The location to check. </param>
		///
		/// <returns>	The creature at location or null if no creature there. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal Creature CreatureAt(MapCoordinates location)
		{
			// Find a creature, if any, at the destination
			return this[location].Items.FirstOrDefault(i => ItemInfo.GetItemInfo(i).IsCreature) as Creature;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	querty if there's a creature at a particular location. </summary>
		///
		/// <remarks>	Darrellp, 10/15/2011. </remarks>
		///
		/// <param name="location">	The location to check. </param>
		///
		/// <returns>	true if creature, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal bool IsCreatureAt(MapCoordinates location)
		{
			return CreatureAt(location) != null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Returns true if we want the creature to continue running in this direction. </summary>
		///
		/// <remarks>	Darrellp, 10/15/2011. </remarks>
		///
		/// <param name="location">	The location to be checked. </param>
		///
		/// <returns>	true if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal bool ValidRunningMove(MapCoordinates location)
		{
			return ValidTerrainForMove(location) && !IsCreatureAt(location);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Marks the newly lit and formerly lit spots on the map. </summary>
		///
		/// <remarks>	Darrellp, 10/15/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void Relight()
		{
			foreach (var newlyLitLocation in _fov.NewlySeen)
			{
				this[newlyLitLocation].LitState = LitState.InView;
			}
			foreach (var previouslyLitLocation in _fov.NowUnseen)
			{
				this[previouslyLitLocation].LitState = LitState.Remembered;
			}
		}

			////////////////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>	Move creature to a new location. </summary>
			///
			/// <remarks>	Darrellp, 10/15/2011. </remarks>
			///
			/// <param name="creature">					The creature. </param>
			/// <param name="newLocation">				The new location. </param>
			/// <param name="firstTimeHeroPlacement">	true when placing the hero the first time. </param>
			/// <param name="run">						true when this is part of a run. </param>
			/// <param name="litAtStartOfRun">			A list of lit locations at the start of a run. </param>
			////////////////////////////////////////////////////////////////////////////////////////////////////
		public void MoveCreatureTo(
			Creature creature,
			MapCoordinates newLocation,
			bool firstTimeHeroPlacement = false,
			bool run = false, 
			List<MapCoordinates> litAtStartOfRun = null)
		{
			// Get the data from the current location
			MapLocationData data = this[creature.Location];
			MapCoordinates oldPosition = creature.Location;

			// Remove the creature from this location
			data.RemoveItem(creature);

			// Place the creature at the new location
			creature.Location = newLocation;
			this[creature.Location].AddItem(creature);

			// If it's the player and there's a FOV to be calculated
			if (creature.IsPlayer && !run && _fov != null)
			{
				// Rescan for FOV
				_fov.Scan(HeroPosition);
				Relight();
			}

			// If we've got a game object
			if (_game != null)
			{
				// Invoke the move event through it
				_game.InvokeEvent(EventType.CreatureMove, this,
					new CreatureMoveEventArgs(
						this, 
						creature, 
						oldPosition, 
						newLocation, 
						firstTimeHeroPlacement,
						isBlocked:false,
						isRunning:run, 
						litAtStartOfRun:litAtStartOfRun));
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Notify the user that the player was blocked. </summary>
		///
		/// <remarks>	Darrellp, 10/15/2011. </remarks>
		///
		/// <param name="creature">			The creature. </param>
		/// <param name="blockedLocation">	The blocked location. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void NotifyOfBlockage(Creature creature, MapCoordinates blockedLocation)
		{
			_game.InvokeEvent(EventType.CreatureMove, this,
				new CreatureMoveEventArgs(
					this,
					creature,
					creature.Location,
					blockedLocation,
					isBlocked:true));
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Adds a room to our room list. </summary>
		///
		/// <remarks>	Darrellp, 10/6/2011. </remarks>
		///
		/// <param name="groom">	The generic room to be added. </param>
		///
		/// <returns>	true if the room has already been added, false if it hasn't. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal bool AddRoom(GenericRoom groom)
		{
			return Rooms.Add(groom);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Sets the FOV for this map. </summary>
		///
		/// <remarks>	Darrellp, 10/6/2011. </remarks>
		///
		/// <param name="rowCount">			Range of the light field. </param>
		/// <param name="playerLocation">	The player location. </param>
		/// <param name="filter">			A filter to filter out terrain that doesn't belong. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void SetFov(int rowCount, MapCoordinates playerLocation, Func<MapCoordinates, MapCoordinates, bool> filter = null)
		{
			_fov = new FOV(this, rowCount, filter);
			_fov.Scan(playerLocation);
			Relight();
		}
		#endregion

		#region Positional information
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Place terrain on the map. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="iRow">			The row to place the terrain on. </param>
		/// <param name="iCol">			The col to place the terrain on. </param>
		/// <param name="terrainType">	Type of the terrain. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void PlaceTerrain(int iCol, int iRow, TerrainType terrainType)
		{
			if (iCol < 0 || iRow < 0 || iCol >= _map.Length || iRow >= _map[0].Length)
			{
				throw new ArgumentException("Out of bounds in PlaceTerrain");
			}
			_map[iCol][iRow].Terrain = terrainType;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Place terrain on the map. </summary>
		///
		/// <remarks>	Darrellp, 9/22/2011. </remarks>
		///
		/// <param name="location">		The location to place the terrain. </param>
		/// <param name="terrainType">	Type of the terrain. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void PlaceTerrain(MapCoordinates location, TerrainType terrainType)
		{
			PlaceTerrain(location.Column, location.Row, terrainType);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Drops an item in the map. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="iRow">	The row to drop in. </param>
		/// <param name="iCol">	The col to drop in. </param>
		/// <param name="item">	The item to drop. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void Drop(int iCol, int iRow, Item item)
		{
			_map[iCol][iRow].AddItem(item);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Drops an item in the map. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="location">	The location. </param>
		/// <param name="item">		The item to drop. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void Drop(MapCoordinates location, Item item)
		{
			Drop(location.Column, location.Row, item);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Removes an item from the map. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="iRow">	The row to remove from. </param>
		/// <param name="iCol">	The col to remove from. </param>
		/// <param name="item">	The item to remove. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void Remove(int iCol, int iRow, Item item)
		{
			_map[iCol][iRow].RemoveItem(item);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Get the list of items at a position. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="iRow">	The row. </param>
		/// <param name="iCol">	The column. </param>
		///
		/// <returns>	. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal List<Item> Items(int iCol, int iRow)
		{
			return _map[iCol][iRow].Items;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the terrain type at a position. </summary>
		///
		/// <remarks>	Darrellp, 9/16/2011. </remarks>
		///
		/// <param name="iRow">	The row. </param>
		/// <param name="iCol">	The column. </param>
		///
		/// <returns>	Terrain type at the position. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal TerrainType Terrain(int iCol, int iRow)
		{
			return _map[iCol][iRow].Terrain;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the terrain type at a position. </summary>
		///
		/// <remarks>	Darrellp, 9/25/2011. </remarks>
		///
		/// <param name="location">	The location to place the terrain. </param>
		///
		/// <returns>	Terrain type at the position. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal TerrainType Terrain(MapCoordinates location)
		{
			return Terrain(location.Column, location.Row);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return all neighbors of a cell taking the borders into account. </summary>
		///
		/// <remarks>	Darrellp, 9/26/2011. </remarks>
		///
		/// <param name="column">	The location's columns. </param>
		/// <param name="row">		The location's row. </param>
		///
		/// <returns>	An enumerable of all neighbors. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal IEnumerable<MapCoordinates> Neighbors(int column, int row)
		{
			int minRowOffset = row == 0 ? 0 : -1;
			int maxRowOffset = row == Height - 1 ? 0 : 1;
			int minColumnOffset = column == 0 ? 0 : -1;
			int maxColumnOffset = column == Width - 1 ? 0 : 1;

			for (int iRowOffset = minRowOffset; iRowOffset <= maxRowOffset; iRowOffset++)
			{
				for (int iColumnOffset = minColumnOffset; iColumnOffset <= maxColumnOffset; iColumnOffset++)
				{
					if (iRowOffset != 0 || iColumnOffset != 0)
					{
						yield return new MapCoordinates(column + iColumnOffset, row + iRowOffset);
					}
				}
			}
		}

		internal IEnumerable<MapCoordinates> Neighbors(MapCoordinates location)
		{
			return Neighbors(location.Column, location.Row);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Find a random floor location. </summary>
		///
		/// <remarks>	Darrellp, 10/3/2011. </remarks>
		///
		/// <returns>	The location found. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public MapCoordinates RandomFloorLocation(bool restrictToRooms = false)
		{
			// Locals
			Rnd rnd = Rnd.Global;
			int row, column;
			MapLocationData data;

			do
			{
				// Try a random spot
				row = rnd.Next(Height);
				column = rnd.Next(Width);
				data = this[column, row];
			}
			// We find one that's on some floor terrain
			while (data.Terrain != TerrainType.Floor && (!restrictToRooms || !data.Room.IsCorridor));

			// Return it
			return new MapCoordinates(column, row);
		}

		public bool Contains(int column, int row)
		{
			return column >= 0 && row >= 0 && column < Width && row < Height;
		}

		public bool Contains(MapCoordinates location)
		{
			return Contains(location.Column, location.Row);
		}

		#endregion

		#region Display
		public override string ToString()
		{
			// Initialize a stringbuilder
			StringBuilder sb = new StringBuilder();

			// For each row of the map
			for (int iRow = 0; iRow < Height; iRow++)
			{
				// For each column of the map
				for (int iColumn = 0; iColumn < Width; iColumn++)
				{
					// Retrieve the data
					MapLocationData data = _map[iColumn][iRow];

					// Are there items here?
					if (data.Items.Count != 0)
					{
						// Draw the first item's character
						ItemInfo info = ItemInfo.GetItemInfo(data.Items[0]);
						sb.Append(info.Character);
					}
					else
					{
						// Draw the terrain character
						sb.Append(TerrainFactory.TerrainToChar(_map[iColumn][iRow].Terrain));
					}
				}

				// Write CRLF to advance to next line
				sb.Append("\r\n");
			}
			return sb.ToString();
		}
		#endregion
	}
}