using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSRogue.GameControl.Commands;
using CSRogue.Map_Generation;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Game;
using SadConsole.Consoles;
using CSRogue.Utilities;
using RogueSC.Commands;
using RogueSC.Utilities;
using static RogueSC.Map_Objects.ScRender;
using Console = SadConsole.Consoles.Console;
using Game = CSRogue.GameControl.Game;

namespace RogueSC.Consoles
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   The console that actually displays the dungeon. </summary>
    ///
    /// <remarks>   Darrellp, 8/26/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class DungeonMapConsole : Console
    {
        #region Public Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the playerSprite GameObject. </summary>
        ///
        /// <remarks>
        /// Note that this is the SadConsole version of the player - i.e., the SadConsole sprite that
        /// represents the player.  It's only purpose is to get drawn on the screen and it has no actual
        /// player information contained in it (other than position I suppose).  Other player information
        /// is stored in the CSRogue object located at _map.Player.
        /// </remarks>
        ///
        /// <value> The player sprite. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal GameObject PlayerSprite => _playerSprite;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the distance we see in the field of view. </summary>
        ///
        /// <value> The fov distance. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal int FovDistance { get; set; } = 8;
        #endregion

        #region Private Variables
        /// <summary>   The CSRogue map. </summary>
        private SCMap _map;

        private GameObject _playerSprite;

        /// <summary>   The CSRogue game object. </summary>
        /// <remarks> The Game object queues up commands and dispatches them and also is a central
        ///           repository about all the information in the game such as the current level, etc. </remarks>
        private readonly Game _game;
        #endregion

        #region Constructor
        /// <summary>   Size to multiply by for the different font sizes. </summary>
        private static readonly Dictionary<Font.FontSizes, double> SizeMultipliers = new Dictionary<Font.FontSizes, double>()
        {
            {Font.FontSizes.Quarter, 0.25},
            {Font.FontSizes.Half, 0.5},
            {Font.FontSizes.One, 1.0},
            {Font.FontSizes.Two, 2.0},
            {Font.FontSizes.Three, 3.0},
            {Font.FontSizes.Four, 4.0}
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ///
        /// <param name="game">         The game. </param>
        /// <param name="viewWidth">    Width of the console. </param>
        /// <param name="viewHeight">   Height of the console. </param>
        /// <param name="fontSize">     (Optional) size of the font. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal DungeonMapConsole(Game game, int viewWidth, int viewHeight, Font.FontSizes fontSize = Font.FontSizes.One) 
            : base(game.Map.Width, game.Map.Height)
        {
            _game = game;

            // Hook up CSRogue command callbacks
            _game.HeroMoveEvent += _game_HeroMoveEvent;
            _game.CreatureMoveEvent += _game_CreatureMoveEvent;
            _game.AttackEvent += _game_AttackEvent;

            // The ToogleDoorEvent is located in our SadConsole app rather than the engine
            ToggleDoorCommand.ToggleDoorEvent += _game_ToggleDoorEvent;

            // Change the font to a square one for the dungeon
            var fontMaster = Engine.LoadFont("Cheepicus12.font");
            var font = fontMaster.GetFont(fontSize);
            TextSurface.Font = font;

            // Determine the height/width of the console in the new font size
            var fontsizeStats = Engine.LoadFont("IBM.font").GetFont(Font.FontSizes.One).Size;
            var mult = SizeMultipliers[fontSize];
            var glyphHeight = fontsizeStats.Y * viewHeight;
            var glyphWidth = fontsizeStats.X * viewWidth;
            var charHeight = (int)(glyphHeight * mult / font.Size.Y);
            var charWidth = (int)(glyphWidth  * mult / font.Size.X);

            // Set the size of the dungeon render area
            TextSurface.RenderArea = new Rectangle(0, 0, charWidth, charHeight);

            NewLevelInit();
        }

        internal void NewLevelInit()
        {
            Clear();

            // Set up the sprite for the player
            var playerAnimation = new AnimatedTextSurface("default", 1, 1, TextSurface.Font);
            playerAnimation.CreateFrame();
            playerAnimation.CurrentFrame[0].Foreground = Color.Orange;
            playerAnimation.CurrentFrame[0].GlyphIndex = 1; //'@';

            _playerSprite = new GameObject(TextSurface.Font)
            {
                Animation = playerAnimation,
                Position = new Point(1, 1)
            };

            GenerateMap();
        }

        #endregion

        #region Event handlers
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by _game for attack events. </summary>
        ///
        /// <remarks>   Darrell, 9/16/2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Attack event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void _game_AttackEvent(object sender, CSRogue.RogueEventArgs.AttackEventArgs e)
        {
            if (e.Victim.IsPlayer)
            {
                return;
            }
            var loc = e.Victim.Location;
            RenderToCell(_map.GetAppearance(loc), this[loc.Column, loc.Row], true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by _game for toggle door events. </summary>
        ///
        /// <remarks>   Darrell, 9/16/2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Toogle door event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void _game_ToggleDoorEvent(object sender, ToogleDoorEventArgs e)
        {
            // Update the SadConsole console if the door is within our FOV
            if (_map.InView(e.DoorLocation))
            {
                RenderToCell(_map.GetAppearance(e.DoorLocation), this[e.DoorLocation.Column, e.DoorLocation.Row], true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by _game for non-player creature movement. </summary>
        ///
        /// <remarks>   The player is handled by an onscreen sprite but monsters have to be hand rendered
        ///             whenever they move around while in the field of view.  Darrell, 9/16/2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Creature move event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void _game_CreatureMoveEvent(object sender, CSRogue.RogueEventArgs.CreatureMoveEventArgs e)
        {
            if (e.IsBlocked || e.IsFirstTimePlacement)
            {
                return;
            }

            // Check on whether his previous location needs rendering
            var loc = e.PreviousCreatureLocation;
            if (_map.InView(loc))
            {
                RenderToCell(_map.GetAppearance(loc), this[loc.Column, loc.Row], true);
            }

            // Check on whether his current location needs rendering
            loc = e.CreatureDestination;
            if (_map.InView(loc))
            {
                RenderToCell(_map.GetAppearance(loc), this[loc.Column, loc.Row], true);
            }
        }
        #endregion

        #region Mapping
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a map. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void GenerateMap()
        {
            _map = (SCMap)_game.Map;

            // Loop through the map information generated by CSRogue and create our cached visuals of that data
            for (var iCol = 0; iCol < Width; iCol++)
            {
                for (var iRow = 0; iRow < Height; iRow++)
                {
                    var terrain = _map[iCol, iRow].Terrain;
                    if (terrain == TerrainType.OffMap)
                    {
                        continue;
                    }
                    _map.GetAppearance(iCol, iRow).CopyAppearanceTo(this[iCol, iRow]);
                    RemoveCellFromView(this[iCol, iRow]);
                }
            }

            PlayerSprite.Position = _map.Player.Location.ToPoint();

            // Center the view area
            TextSurface.RenderArea = new Rectangle(PlayerSprite.Position.X - (TextSurface.RenderArea.Width / 2),
                                                    PlayerSprite.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                    TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

            PlayerSprite.RenderOffset = Position - TextSurface.RenderArea.Location;
			Refresh();
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Refreshes the viewed map. </summary>
        ///
        /// <remarks>   Similar to the PlayerMoved event handling but no map movement and we draw everything
        ///             the player currently sees rather than just the newly seen stuff.  Darrell, 9/16/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
		private void Refresh()
	    {
		    _map.ScanPlayer();

		    foreach (var loc in _map.Fov.CurrentlySeen())
		    {
				RenderToCell(_map.GetAppearance(loc), this[loc.Column, loc.Row], true);
		    }
		    foreach (var loc in _map.Fov.NewlyUnseen())
		    {
				RenderToCell(_map.GetAppearance(loc), this[loc.Column, loc.Row], false);
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Toggle doors adjacent to the player. </summary>
        ///
        /// <remarks>   Darrell, 9/16/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void ToggleDoors()
        {
            foreach (var doorLoc in _map.Neighbors(_map.Player.Location).Where(l => _map[l].Terrain == TerrainType.Door))
            {
                if (_map[doorLoc].Items.Count == 0)
                {
                    // Set up the door toggle command and queue it up.
                    var cmd = new ToggleDoorCommand(_map.Player, doorLoc);
                    _game.Enqueue(cmd);
                }
            }

            // This will give the monsters a chance to move and also actually fire off all our queued up door commands
            MovePlayerBy(new Point(0, 0));
        }
        #endregion

        #region Player handling
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Move player by a delta. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ///
        /// <param name="amount">   The delta to move the player by. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void MovePlayerBy(Point amount)
        {
            // Move the player on the engine map
            var moveCmd = new MovePlayerCommand(amount.ToMapCoordinates());
            _game.EnqueueAndProcess(moveCmd);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler called by game for hero move events. </summary>
        ///
        /// <remarks>   This is where we react to any player movement and update the screen
        ///             Darrell, 9/9/2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Creature move event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void _game_HeroMoveEvent(object sender, CSRogue.RogueEventArgs.CreatureMoveEventArgs e)
        {
            // If we were blocked or this is the first time we've been placed then there is no real
            // "movement" and we can just forget the whole thing.
            if (e.IsBlocked || e.IsFirstTimePlacement)
            {
                return;
            }

            // Move the SadConsole sprite to the destination
            PlayerSprite.Position = e.CreatureDestination.ToPoint();

            // Scroll the view area to center the player on the screen
            TextSurface.RenderArea = new Rectangle(PlayerSprite.Position.X - (TextSurface.RenderArea.Width / 2),
                                                    PlayerSprite.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                    TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

            // If he view area moved, we'll keep our entity in sync with it.
            PlayerSprite.RenderOffset = Position - TextSurface.RenderArea.Location;

            // Update any positions on the screen that have been revealed or hidden
            foreach (var loc in e.GameMap.Fov.NewlySeen())
            {
                RenderToCell(
					_map.GetAppearance(loc),
                    this[loc.Column, loc.Row],
                    true);
            }
            foreach (var loc in e.GameMap.Fov.NewlyUnseen())
            {
                RenderToCell(
                    _map.GetAppearance(loc),
                    this[loc.Column, loc.Row],
                    false);
            }
        }

        public override void Render()
        {
            base.Render();
            PlayerSprite.Render();
        }

        public override void Update()
        {
            base.Update();
            PlayerSprite.Update();
        }
        #endregion

        #region Debugging
        [Conditional("DEBUG")]
        internal void CheckFOV()
        {
            foreach (var seenLoc in _map.Fov.CurrentlySeen())
            {
                // Player is represented with a sprite which isn't represented directly in the
                // DungeonMapConsole so we skip that.
                if (seenLoc == _map.Player.Location)
                {
                    continue;
                }
                var expectedGlyph = _map.GetAppearance(seenLoc).GlyphIndex;
                var cell = this[seenLoc.Column, seenLoc.Row];
                if (cell.GlyphIndex != expectedGlyph)
                {
                    throw new InvalidOperationException("Mismatch between seen map and CSRogue map");
                }
            }
        }
        #endregion
    }
}
