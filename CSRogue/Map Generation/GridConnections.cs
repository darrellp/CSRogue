using System.Collections.Generic;
using System.Linq;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	class GridConnections
	{
		#region Private variables
		private readonly bool[,] _connectsRight;
		private readonly bool[,] _connectsDown;
		private readonly int _mapWidth;
		private readonly int _mapHeight;
		private int _freeConnectionCount;
		#endregion

		#region Internal Properties
		internal MapCoordinates FirstRoomConnected { get; private set; }
		internal MapCoordinates LastRoomConnected { get; private set; }
		#endregion

		#region Constructor
		internal GridConnections(int width, int height)
		{
			_connectsRight = new bool[width - 1, height];
			_connectsDown = new bool[width, height - 1];
			_mapWidth = width;
			_mapHeight = height;
			_freeConnectionCount = (width - 1) * height + width * (height - 1);
		} 
		#endregion

		#region Setting connections
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Connects two adacent cells vertically. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="column">		The column the cells occupy. </param>
		/// <param name="higherRow">	The row of the higher cell - i.e., the row with the smaller row index. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void ConnectVertical(int column, int higherRow)
		{
			_connectsDown[column, higherRow] = true;
			_freeConnectionCount--;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Connects two adacent cells horizontally. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="row">				The row the cells occupy. </param>
		/// <param name="leftmostColumn">	The column of the leftmost cell. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void ConnectHorizontal(int row, int leftmostColumn)
		{
			_connectsRight[leftmostColumn, row] = true;
			_freeConnectionCount--;
		} 
		#endregion

		#region Queries
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Check coordinates to see if they're neighbors. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="coord1">		[in,out] The first coordinate - upon return, the coordinate with the smallest value. </param>
		/// <param name="coord2">		[in,out] The second coordinate - upon return, the coordinate with the largest value. </param>
		/// <param name="fVertical">	[out] Returns as true if the positions are vertical neighbors. </param>
		///
		/// <returns>	true if the locations are neighbors - false otherwise. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static bool CheckCoordinates(ref MapCoordinates coord1, ref MapCoordinates coord2, out bool fVertical)
		{
			// If they're in the same column then they're vertical so set the fVertical flag
			fVertical = coord1.Column == coord2.Column;

			// Are they in different rows and different columns?
			if (!fVertical && coord1.Row != coord2.Row)
			{
				// return false - they're not neighbors
				return false;
			}

			// Determine if they need to be swapped
			bool fSwap = fVertical ? coord2.Row < coord1.Row : coord2.Column < coord1.Column;

			// Do they?
			if (fSwap)
			{
				// Swap them
				MapCoordinates temp = coord1;
				coord1 = coord2;
				coord2 = temp;
			}

			// return true if their differing coordinates differ by one
			int coordinateDifference = fVertical ? coord2.Row - coord1.Row : coord2.Column - coord1.Column;
			return coordinateDifference == 1;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Makes a random connection in the grid. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <exception cref="RogueException">	Thrown when there are no remaining connections to be made. </exception>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void MakeRandomConnection(Rnd rnd)
		{
			// If there are no free connections
			if (_freeConnectionCount == 0)
			{
				// Throw an exception
				throw new RogueException("Searching for an empty connection in a grid where there are none");
			}

			// Pick a random index for the new connection to be made
			int connectionIndex = rnd.Next(_freeConnectionCount);

			foreach (var info in Connections.Where(info => !info.IsConnected && connectionIndex-- == 0))
			{
				info.SetValue(true);
				break;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Ensure that all rooms are connected. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void ConnectCells(Rnd rnd)
		{
			// locals
			List<MapCoordinates> unconnectedCellList = new List<MapCoordinates>();
			bool[,] isConnected = new bool[_mapWidth, _mapHeight];

			// Initialize to all cleared connections
			ClearConnections();

			// For each column in grid
			for (int iColumn = 0; iColumn < _mapWidth; iColumn++)
			{
				// For each row in grid
				for (int iRow = 0; iRow < _mapHeight; iRow++)
				{
					// Mark the grid cell as unconnected
					unconnectedCellList.Add( new MapCoordinates(iColumn, iRow));
				}
			}

			// Allocate a list of connected cells
			List<MapCoordinates> connectedCells = new List<MapCoordinates>(unconnectedCellList.Count);

			// Pick out an intially "connected" cell
			int cellIndex = rnd.Next(unconnectedCellList.Count);
			MapCoordinates connectedCell = unconnectedCellList[cellIndex];
			isConnected[connectedCell.Column, connectedCell.Row] = true;

			// Add it to the connected list
			connectedCells.Add(connectedCell);
			FirstRoomConnected = connectedCell;

			// Remove it from the unconnected list
			unconnectedCellList.RemoveAt(cellIndex);

			// While there are unconnected cells
			while (unconnectedCellList.Count != 0)
			{
				// Pick a random connected cell
				int connectedCellIndex = rnd.Next(connectedCells.Count);
				connectedCell = connectedCells[connectedCellIndex];

				// Get the totally unconnected neighbors it's not connected with
				List<MapCoordinates> unconnectedNeighbors =
					UnconnectedNeighbors(connectedCell).
						Where(crd => !isConnected[crd.Column, crd.Row]).ToList();

				// Are there any such neighbors?
				if (unconnectedNeighbors.Count != 0)
				{
					// Pick a random such neighbor
					int neighborIndex = rnd.Next(unconnectedNeighbors.Count);
					MapCoordinates neighborLocation = unconnectedNeighbors[neighborIndex];
					LastRoomConnected = neighborLocation;

					// Connect it to our already connected cell
					Connect(connectedCell, neighborLocation);

					// Take the neighbor off the unconnected list
					unconnectedCellList.Remove(neighborLocation);

					// Add it to the connected list
					connectedCells.Add(neighborLocation);

					// and mark it as connected
					isConnected[neighborLocation.Column, neighborLocation.Row] = true;
				}
				else
				{
					// Take the connected cell off the connected list
					// It doesn't do us any good any more since it's a dead end
					connectedCells.RemoveAt(connectedCellIndex);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Query if two positions are connected. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="coord1">	The first coordinate. </param>
		/// <param name="coord2">	The second coordinate. </param>
		///
		/// <returns>	true if connected, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal bool IsConnected(MapCoordinates coord1, MapCoordinates coord2)
		{
			bool fVertical;
			bool ret = false;

			// Are they valid neighbors?
			if (CheckCoordinates(ref coord1, ref coord2, out fVertical))
			{
				// Choose the correct connection array based on fVertical
				bool[,] connectArray = fVertical ? _connectsDown : _connectsRight;

				// See if they're actually connected
				ret = connectArray[coord1.Column, coord1.Row];
			}
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Connects two adjacent locations. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="coord1">	The first coordinate. </param>
		/// <param name="coord2">	The second coordinate. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void Connect(MapCoordinates coord1, MapCoordinates coord2)
		{
			bool fVertical;

			// Are they valid neighbors?
			if (CheckCoordinates(ref coord1, ref coord2, out fVertical))
			{
				// If vertical
				if (fVertical)
				{
					// Make the vertical connection
					ConnectVertical(coord1.Column, coord1.Row);
				}
				else
				{
					// Make the horizontal connection
					ConnectHorizontal(coord1.Row, coord1.Column);
				}
			}
			else
			{
				// Throw an exception
				throw new RogueException("Trying to connect unconnected locations in GridConnections.Connect()");
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Returns information on all the connections in the grid. </summary>
		///
		/// <value>	ConnectionInfo for each of the locations in the grid </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal IEnumerable<ConnectionInfo> Connections
		{
			get
			{
				// For each row
				for (int iRow = 0; iRow < _mapHeight - 1; iRow++)
				{
					// For each column
					for (int iColumn = 0; iColumn < _mapWidth; iColumn++)
					{
						yield return new ConnectionInfo(this, new MapCoordinates(iColumn, iRow), Dir.Vert, _connectsDown[iColumn, iRow]);
					}
				}

				// For each row
				for (int iRow = 0; iRow < _mapHeight; iRow++)
				{
					// For each column
					for (int iColumn = 0; iColumn < _mapWidth - 1; iColumn++)
					{
						yield return new ConnectionInfo(this, new MapCoordinates(iColumn, iRow), Dir.Horiz, _connectsRight[iColumn, iRow]);
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Clears all connections. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void ClearConnections()
		{
			foreach (var info in Connections)
			{
				info.SetValue(false);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return the unconnected neighbors of a position. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="cellLocation">	The cell location whose unconnected neighbors are desired. </param>
		///
		/// <returns>	The unconnected neighbors as an IEnumerable. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal IEnumerable<MapCoordinates> UnconnectedNeighbors(MapCoordinates cellLocation)
		{
			return cellLocation.Neighbors(_mapWidth, _mapHeight).Where(crd => !IsConnected(crd, cellLocation));
		}
		#endregion

		#region Internal classes
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Information about connections. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal struct ConnectionInfo
		{
			#region Private variables
			private readonly GridConnections _connections;
		    private readonly Dir _dir;
			#endregion

			#region Properties
			internal MapCoordinates Location { get; }
		    internal Dir Dir => _dir;
		    internal bool IsConnected { get; private set; }
			#endregion

			#region Constructor
			public ConnectionInfo(GridConnections connections, MapCoordinates location, Dir dir, bool isConnected) : this()
			{
				Location = location;
				_dir = dir;
				IsConnected = isConnected;
				_connections = connections;
			} 
			#endregion

			#region Modification
			internal void SetValue(bool isConnectedNew)
			{
				// If we haven't changed the value, then there's nothing to do
				if (isConnectedNew == IsConnected)
				{
					return;
				}

				if (_dir == Dir.Vert)
				{
					_connections._connectsDown[Location.Column, Location.Row] = isConnectedNew;
				}
				else
				{
					_connections._connectsRight[Location.Column, Location.Row] = isConnectedNew;
				}
				_connections._freeConnectionCount += IsConnected ? 1 : -1;
				IsConnected = isConnectedNew;
			} 
			#endregion
		} 
		#endregion
	}
}
