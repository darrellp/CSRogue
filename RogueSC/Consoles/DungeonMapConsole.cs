using System.Collections.Generic;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Game;
using SadConsole.Consoles;
using CSRogue.Map_Generation;
using CSRogue.Utilities;
using RogueSC.Map_Objects;
using RogueSC.Utilities;
using Console = SadConsole.Consoles.Console;

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
        private CsRogueMap _csRogueMap;

        /// <summary>   Information describing the map.  Not sure that this isn't
        ///             subsumed by the _map information.  Probably is. </summary>
        MapObject[,] _mapData;

        /// <summary>   The field of view for the map. </summary>
        private FOV _fov;
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
        /// <param name="viewWidth">    Width of the console. </param>
        /// <param name="viewHeight">   Height of the console. </param>
        /// <param name="mapWidth">     Width of the underlying map. </param>
        /// <param name="mapHeight">    Height of the underlying map. </param>
        /// <param name="fontSize">     (Optional) size of the font. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DungeonMapConsole(int viewWidth, int viewHeight, int mapWidth, int mapHeight, Font.FontSizes fontSize = Font.FontSizes.One) 
            : base(mapWidth, mapHeight)
        {
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

        #region Mapping

        /// <summary>   Maps terrain types to appearance for that terrain. </summary>
        private static readonly Dictionary<TerrainType, string> MapTerrainToAppearance = new Dictionary
            <TerrainType, string>()
        {
            {TerrainType.Floor, "floor"},
            {TerrainType.Door, "floor"},
            {TerrainType.StairsDown, "floor"},
            {TerrainType.StairsUp, "floor"},
            {TerrainType.Corner, "wall"},
            {TerrainType.HorizontalWall, "wall"},
            {TerrainType.VerticalWall, "wall"},
            {TerrainType.Wall, "wall"},
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a map. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GenerateMap()
        {
            _csRogueMap = new CsRogueMap(Width, Height);
            var excavator = new GridExcavator();
            excavator.Excavate(_csRogueMap);
            // Create the local cache of map data
            // 
            _mapData = new MapObject[Width, Height];

            // Loop through the map information generated by RogueSharp and create our cached visuals of that data
            for (var iCol = 0; iCol < Width; iCol++)
            {
                for (var iRow = 0; iRow < Height; iRow++)
                {
                    var terrain = _csRogueMap[iCol, iRow].Terrain;
                    if (terrain == TerrainType.OffMap)
                    {
                        continue;
                    }
                    string str = MapTerrainToAppearance[_csRogueMap[iCol, iRow].Terrain];
                    var obj = _mapData[iCol, iRow] = new MapObject(MapObjectFactory.ObjectNameToAppearance[str]);
                    obj.Appearance.CopyAppearanceTo(this[iCol, iRow]);
                    obj.RemoveCellFromView(this[iCol, iRow]);
                }
            }

            Player.Position = _csRogueMap.RandomFloorLocation().ToPoint();

            // Center the veiw area
            TextSurface.RenderArea = new Rectangle(Player.Position.X - (TextSurface.RenderArea.Width / 2),
                                                    Player.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                    TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

            Player.RenderOffset = Position - TextSurface.RenderArea.Location;
            _fov = new FOV(_csRogueMap, FovDistance);
            _fov.Scan(Player.Position.ToMapCoordinates());
            foreach (var loc in _fov.CurrentlySeen)
            {
                _mapData[loc.Column, loc.Row].RenderToCell(this[loc.Column, loc.Row], true);
            }
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
            // Get the position the player will be at
            var newPosition = Player.Position + amount;

            // Check to see if the position is within the map
            if (new Rectangle(0, 0, Width, Height).Contains(newPosition) && _csRogueMap.Walkable(newPosition.X, newPosition.Y))
            {
                // Move the player
                Player.Position += amount;
                _csRogueMap.Player.Location = Player.Position.ToMapCoordinates();

                // Scroll the view area to center the player on the screen
                TextSurface.RenderArea = new Rectangle(Player.Position.X - (TextSurface.RenderArea.Width / 2),
                                                        Player.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                        TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

                // If he view area moved, we'll keep our entity in sync with it.
                Player.RenderOffset = Position - TextSurface.RenderArea.Location;

                _fov.Scan(Player.Position.ToMapCoordinates());
                foreach (var loc in _fov.NewlySeen)
                {
	                _mapData[loc.Column, loc.Row].RenderToCell(
		                this[loc.Column, loc.Row],
		                true);
                }
                foreach (var loc in _fov.NewlyUnseen)
                {
                    _mapData[loc.Column, loc.Row].RenderToCell(
                        this[loc.Column, loc.Row],
                        false);
                }
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
