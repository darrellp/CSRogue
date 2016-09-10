using System.Collections.Generic;
using CSRogue.GameControl.Commands;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Game;
using SadConsole.Consoles;
using CSRogue.Map_Generation;
using CSRogue.Utilities;
using RogueSC.Utilities;
using static RogueSC.Map_Objects.SCRender;
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
        /// <summary>   Gets the player GameObject. </summary>
        ///
        /// <value> The player. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GameObject Player { get; }

        public int FovDistance { get; set; } = 8;
        #endregion

        #region Private Variables
        /// <summary>   The CSRogue map. </summary>
        private IGameMap _map;

        // The engine game
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

        public DungeonMapConsole(Game game, int viewWidth, int viewHeight, Font.FontSizes fontSize = Font.FontSizes.One) 
            : base(game.Map.Width, game.Map.Height)
        {
            _game = game;
            _game.HeroMoveEvent += _game_HeroMoveEvent;
            _game.CreatureMoveEvent += _game_CreatureMoveEvent;
            _game.AttackEvent += _game_AttackEvent;
            var fontMaster = Engine.LoadFont("Cheepicus12.font");
            var font = fontMaster.GetFont(fontSize);
            TextSurface.Font = font;
            var mult = SizeMultipliers[fontSize];
            TextSurface.RenderArea = new Rectangle(0, 0, (int)(viewWidth * 2 / (3 * mult)), (int)(viewHeight * 3 / (2 * mult)) - 1);

            var playerAnimation = new AnimatedTextSurface("default", 1, 1, font);
            playerAnimation.CreateFrame();
            playerAnimation.CurrentFrame[0].Foreground = Color.Orange;
            playerAnimation.CurrentFrame[0].GlyphIndex = 1;//'@';

            Player = new GameObject(font)
            {
                Animation = playerAnimation,
                Position = new Point(1, 1)
            };

            GenerateMap();
        }
        #endregion

        #region Event handlers
        private void _game_AttackEvent(object sender, CSRogue.RogueEventArgs.AttackEventArgs e)
        {
            if (e.Victim.IsPlayer)
            {
                return;
            }
            var loc = e.Victim.Location;
            RenderToCell(GetAppearance(loc), this[loc.Column, loc.Row], true);
        }

        private void _game_CreatureMoveEvent(object sender, CSRogue.RogueEventArgs.CreatureMoveEventArgs e)
        {
            if (e.IsBlocked || e.IsFirstTimePlacement)
            {
                return;
            }
            var loc = e.PreviousCreatureLocation;
            if (_map.InView(loc))
            {
                RenderToCell(GetAppearance(loc), this[loc.Column, loc.Row], true);
            }
            else if (_map.Remembered(loc))
            {
                RenderToCell(FloorAppearance, this[loc.Column, loc.Row], false);
            }
            loc = e.CreatureDestination;
            if (_map.InView(loc))
            {
                RenderToCell(GetAppearance(loc), this[loc.Column, loc.Row], true);
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
            _map = _game.Map;

            // Loop through the map information generated by RogueSharp and create our cached visuals of that data
            for (var iCol = 0; iCol < Width; iCol++)
            {
                for (var iRow = 0; iRow < Height; iRow++)
                {
                    var terrain = _map[iCol, iRow].Terrain;
                    if (terrain == TerrainType.OffMap)
                    {
                        continue;
                    }
                    GetAppearance(iCol, iRow).CopyAppearanceTo(this[iCol, iRow]);
                    RemoveCellFromView(this[iCol, iRow]);
                }
            }

            Player.Position = _map.Player.Location.ToPoint();

            // Center the veiw area
            TextSurface.RenderArea = new Rectangle(Player.Position.X - (TextSurface.RenderArea.Width / 2),
                                                    Player.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                    TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

            Player.RenderOffset = Position - TextSurface.RenderArea.Location;
            foreach (var loc in _map.Fov.CurrentlySeen)
            {
                RenderToCell(GetAppearance(loc.Column, loc.Row), this[loc.Column, loc.Row], true);
            }
        }

        private CellAppearance GetAppearance(MapCoordinates crd)
        {
            return GetAppearance(crd.Column, crd.Row);
        }

        private CellAppearance GetAppearance(int iCol, int iRow)
        {
            CellAppearance appearance;
            if (_map[iCol, iRow].Items.Count > 0)
            {
                var id = _map[iCol, iRow].Items[0].ItemTypeId;
                appearance = id == ItemIDs.HeroId ?
                    MapTerrainToAppearance[_map[iCol, iRow].Terrain] :
                    ObjectNameToAppearance[_game.Factory.InfoFromId[id].Name];
            }
            else
            {
                appearance = MapTerrainToAppearance[_map[iCol, iRow].Terrain];
            }
            return appearance;
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

        public void MovePlayerBy(Point amount)
        {
            // Move the player on the engine map
            var moveCmd = new MovementCommand(amount.ToMapCoordinates());
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
            if (e.IsBlocked || e.IsFirstTimePlacement)
            {
                return;
            }
            Player.Position = e.CreatureDestination.ToPoint();

            // Scroll the view area to center the player on the screen
            TextSurface.RenderArea = new Rectangle(Player.Position.X - (TextSurface.RenderArea.Width / 2),
                                                    Player.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                    TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

            // If he view area moved, we'll keep our entity in sync with it.
            Player.RenderOffset = Position - TextSurface.RenderArea.Location;

            foreach (var loc in e.GameMap.Fov.NewlySeen)
            {
                RenderToCell(
                    GetAppearance(loc.Column, loc.Row),
                    this[loc.Column, loc.Row],
                    true);
            }
            foreach (var loc in e.GameMap.Fov.NewlyUnseen)
            {
                RenderToCell(
                    GetAppearance(loc.Column, loc.Row),
                    this[loc.Column, loc.Row],
                    false);
            }
        }

        public override void Render()
        {
            base.Render();
            Player.Render();
        }

        public override void Update()
        {
            base.Update();
            Player.Update();
        }
        #endregion
    }
}
