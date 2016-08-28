using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;
using Bramble.Core;
using CSRogue.RogueEventArgs;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using Malison.Core;
using Malison.WinForms;
using RogueWPF.Utilities;
using Rect = Bramble.Core.Rect;

namespace RogueWPF.Mapping
{
	class MapHandler
	{
		#region Private Variables
		private const int ScrollAmount = 5;
		private const int RowCount = 70;
		private const int ColumnCount = 160;
		private int _screenRowCount;
		private int _screenColumnCount;
		private readonly Character _spaceChar = new Character(' ');
		private readonly int _charWidth;
		private readonly int _charHeight;
		private readonly WindowsFormsHost _mapHost;
		private readonly MainWindow _mainWindow;
		private Rect _onScreen = new Rect(RowCount, ColumnCount);
		private readonly Game _game;
		private bool _initialResize = true;

		private readonly Terminal _terminal;
		private static readonly Dictionary<TerrainType, Character> CharacterInfo = new Dictionary<TerrainType, Character>
		    {
				{TerrainType.Floor, new Character(Glyph.Period, TermColor.Orange)},
				{TerrainType.Wall, new Character(Glyph.SolidFill, TermColor.DarkBlue)},
				{TerrainType.Door, new Character(Glyph.Door, TermColor.Yellow)},
				{TerrainType.StairsUp, new Character(Glyph.LessThan, TermColor.Yellow)},
				{TerrainType.StairsDown, new Character(Glyph.GreaterThan, TermColor.Yellow)},
			}; 
		#endregion

		#region Properties
		public Map Map
		{
			get
			{
				return _game.CurrentLevel.Map;
			}
		}
		#endregion

		#region Constructor
		public MapHandler(TerminalControl terminalCtl, WindowsFormsHost mapHost, Game game)
		{
			_terminal = new Terminal(ColumnCount, RowCount);
			terminalCtl.Terminal = _terminal;
			_mapHost = mapHost;
			_charWidth = terminalCtl.GlyphSheet.Width;
			_charHeight = terminalCtl.GlyphSheet.Height;
			_game = game;
			_game.HeroMoveEvent += HandleCreatureMoveEvent;
			_game.AttackEvent += GameAttackEvent;
			NewLevelCommand levelCommand = new NewLevelCommand
			    {
			        Width = ColumnCount,
			        Height = RowCount,
			        FOVRows = 20,
			        Filter = (locHero, locSite) => MapCoordinates.Within(locHero, locSite, 15, 10, 12),
			        Level = 0
			    };
			_game.EnqueueAndProcess(levelCommand);
			terminalCtl.Size = terminalCtl.GetPreferredSize(new System.Drawing.Size());
			_mainWindow = (MainWindow)Application.Current.MainWindow;
		}

		void SendMessage(string message)
		{
			_mainWindow.MessageTextblock.Text = message;
		}

		void GameAttackEvent(object sender, AttackEventArgs e)
		{
			string message = string.Empty;
			string monster = string.Empty;

			DrawCell(e.VictimLocation);
			if (e.Attacker == Map.Player)
			{
				monster = ItemInfo.GetItemInfo(e.Victim).Name;
				message = string.Format("You hit the {0}.", monster);
			}
			else if (e.Victim == Map.Player)
			{
				monster = ItemInfo.GetItemInfo(e.Attacker).Name;
				message = string.Format("The {0} hit you.", monster);
			}

			if (e.VictimDied)
			{
				if (e.Victim == Map.Player)
				{
					// We died!!!
				}
				else
				{
					message = message + string.Format("  You killed the {0}!", monster);
				}
			}
			SendMessage(message);
		}

		internal void Resize()
		{
			// Recalculate the size of our map in rows and columns
			RecalculateSize();

			// Is this is the initial resize or have we gone off the screen?
			if (!_onScreen.Inflate(-1).Contains(new Vec(Map.HeroPosition.Column, Map.HeroPosition.Row)) || _initialResize)
			{
				// Center the player on the screen
				CenterPlayer();
				_initialResize = false;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Recalculate row count, column count and the rectangle that is onscreen. </summary>
		///
		/// <remarks>	Darrellp, 10/12/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void RecalculateSize()
		{
			// Do some magic to get pixels and thence rows/columns
			Size sizeScreenMap = PixelConversions.GetElementPixelSize(_mapHost);
			_screenColumnCount = (int)sizeScreenMap.Width / _charWidth;
			_screenRowCount = (int)sizeScreenMap.Height / _charHeight;
			_onScreen = new Rect(_onScreen.Left, _onScreen.Top, _screenColumnCount, _screenRowCount);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Ensures that player is on screen. </summary>
		///
		/// <remarks>	Darrellp, 10/12/2011. </remarks>
		///
		/// <param name="forceRecenter">	true to force a recenter. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void EnsurePlayerIsOnScreen(bool forceRecenter = false)
		{
			// Are we missing a map or a terminal?
			if ((_onScreen.Width == 0) || Map == null || _terminal == null)
			{
				return;
			}

			// Find out where the player is
			MapCoordinates hero = Map.HeroPosition;

			// Are we still on the screen and not asked to recenter?
			if (!forceRecenter && _onScreen.Inflate(-1).Contains(new Vec(hero.Column, hero.Row)))
			{
				return;
			}

			// Set up our scroll parameters
			int dleft = 0;
			int dtop = 0;

			// Are we scrolling off the left side?
			if (hero.Column <= _onScreen.Left + 1)
			{
				// Scroll the map to the right
				dleft = ScrollAmount + _onScreen.Left + 1 - hero.Column;
			}
			// Are we scrolling off the right side?
			else if (hero.Column >= _onScreen.Right - 1)
			{
				// Scroll the map to the left
				dleft = -(ScrollAmount + hero.Column - _onScreen.Right + 1);
			}

			// Are we scrolling off the top?
			if (hero.Row <= _onScreen.Top + 1)
			{
				// Scroll the map down
				dtop = ScrollAmount + _onScreen.Top + 1 - hero.Row;
			}
			// Are we scrolling off the bottom?
			else if (hero.Row >= _onScreen.Bottom - 1)
			{
				// Scroll the map up
				dtop = -(ScrollAmount + hero.Row - _onScreen.Bottom + 1);
			}

			// perform the scroll
			ScrollBy(dleft, dtop);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Center the map on the player. </summary>
		///
		/// <remarks>	Darrellp, 10/12/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void CenterPlayer()
		{
			if (Map == null || _terminal == null)
			{
				return;
			}
			int dleft = _onScreen.Width / 2 - (Map.HeroPosition.Column - _onScreen.Left);
			int dtop = _onScreen.Height / 2 - (Map.HeroPosition.Row - _onScreen.Top);
			ScrollBy(dleft, dtop);
		}

		private void ScrollBy(int dleft, int dtop)
		{
			_onScreen = new Rect(_onScreen.Left - dleft, _onScreen.Top - dtop, _onScreen.Width, _onScreen.Height);
			_terminal.Scroll(dleft, dtop, ScrollCallback);
		}
		#endregion

		#region Map Creation
		internal void InitializeMap()
		{
			DrawMap();
			Resize();
			EnsurePlayerIsOnScreen();
		} 
		#endregion

		#region Drawing
		private void DrawCell(MapCoordinates loc)
		{
			DrawCell(loc.Column, loc.Row);
		}

		private Character ScrollCallback(Vec location)
		{
			int adjustedColumn = location.X + _onScreen.Left;
			int adjustedRow = location.Y + _onScreen.Top;

			return Map.Contains(adjustedColumn, adjustedRow) ? CharacterFromCell(adjustedColumn, adjustedRow) : _spaceChar;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Determine the character and color to draw from the cell contents. </summary>
		///
		/// <remarks>	Darrellp, 10/7/2011. </remarks>
		///
		/// <param name="column">	The column of the cell. </param>
		/// <param name="row">		The row of the cell. </param>
		///
		/// <returns>	A Character of the correct color and glyph for this position on the map. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private Character CharacterFromCell(int column, int row)
		{
			// Get the data at this location
			MapLocationData data = Map[column, row];

			// Are there items here?
			if (data.Items.Count > 0)
			{
				// Are any of them the player?
				if (data.Items.Any(i => ItemInfo.GetItemInfo(i).IsPlayer))
				{
					// Return the smiley face
					return new Character(Glyph.Face, TermColor.White);
				}

				// Otherwise, use the first item in the list
				Item itemCur = data.Items[0];
				ItemInfo info = ItemInfo.GetItemInfo(itemCur);

				// Is it a creature?
				if (info.IsCreature)
				{
					// Get it's color and character and return the glyph
					return new Character(info.Character, (TermColor)(info.CreatureInfo.Color));
				}
			}

			// Do we know how to draw the terrain at this spot?
			if (CharacterInfo.ContainsKey(data.Terrain))
			{
				// Get the character info for this terrain
				Character character = CharacterInfo[data.Terrain];

				// Is this spot lit?
				if (data.LitState == LitState.InView)
				{
					// Change the color to light blue
					character = new Character(character.Glyph, TermColor.LightBlue);
				}

				// Return the character
				return character;
			}

			// If nothing else fits, return a space
			return _spaceChar;
		}
		private void DrawCell(int column, int row)
		{
			// Subtract off onscreen upper left because Terminal doesn't understand scrolling
			int adjustedColumn = column - _onScreen.Left;
			int adjustedRow = row - _onScreen.Top;
			bool inMap = adjustedColumn >= 0 && adjustedRow >= 0;

			// If the coordinates are negative use a space
			// Terminal interprets negative coordinates as "from bottom" or "from right" which is
			// not what we want here - negative coordinates are "off the map" so need to be drawn
			// with a space.
			Character character = inMap ? CharacterFromCell(column, row) : _spaceChar;
			_terminal[adjustedColumn, adjustedRow].Write(character);
		}

		private void DrawMap()
		{
			for (int iColumn = 0; iColumn < Map.Width; iColumn++)
			{
				for (int iRow = 0; iRow < Map.Height; iRow++)
				{
					DrawCell(iColumn, iRow);
				}
			}
		} 
		#endregion

		#region Event handlers
		void HandleCreatureMoveEvent(object sender, CreatureMoveEventArgs e)
		{
			if (e.IsRunning)
			{
				return;
			}
			if (e.IsPlayer)
			{
				if (e.IsBlocked)
				{
					TerrainType terrain = Map[e.CreatureDestination].Terrain;
					SendMessage(string.Format("There is a {0} in the way.", terrain));
				}
				else
				{
					IEnumerable<MapCoordinates> redrawLighting = e.LitAtStartOfRun != null
					    ? e.LitAtStartOfRun.Concat(e.Map.FOV.CurrentlySeen)
					    : e.Map.FOV.NewlySeen.Concat(e.Map.FOV.NewlyUnseen);

					foreach (var newlyLit in redrawLighting)
					{
						DrawCell(newlyLit);
					}
				}
			}
			DrawCell(e.PreviousCreatureLocation);
			DrawCell(e.CreatureDestination);
			if (e.IsPlayer)
			{
				EnsurePlayerIsOnScreen();
			}
		} 
		#endregion
	}
}
