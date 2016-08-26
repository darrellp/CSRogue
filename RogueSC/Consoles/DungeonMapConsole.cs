﻿using System.Collections.Generic;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Game;
using SadConsole.Consoles;
using CSRogue.Map_Generation;
using RoguelikeGame.MapObjects;
using RogueSC.Utilities;
using Console = SadConsole.Consoles.Console;

namespace RogueSC.Consoles
{
    class DungeonMapConsole : Console
    {
        #region Public Properties
        public GameObject Player { get; }
        #endregion

        #region Private Variables
        private Map _map;
        CellAppearance[,] _mapData;
        #endregion

        #region Constructor

        private static Dictionary<Font.FontSizes, double> sizeMultipliers = new Dictionary<Font.FontSizes, double>()
        {
            {Font.FontSizes.Quarter, 0.25},
            {Font.FontSizes.Half, 0.5},
            {Font.FontSizes.One, 1.0},
            {Font.FontSizes.Two, 2.0},
            {Font.FontSizes.Three, 3.0},
            {Font.FontSizes.Four, 4.0}
        };

        public DungeonMapConsole(int viewWidth, int viewHeight, int mapWidth, int mapHeight, Font.FontSizes fontSize = Font.FontSizes.One) 
            : base(mapWidth, mapHeight)
        {
            SadConsole.FontMaster fontMaster = SadConsole.Engine.LoadFont("IBM.font");
            Font font = fontMaster.GetFont(fontSize);
            TextSurface.Font = font;
            var mult = sizeMultipliers[fontSize];
            TextSurface.RenderArea = new Rectangle(0, 0, (int)(viewWidth / mult), (int)(viewHeight / mult));

            AnimatedTextSurface playerAnimation = new AnimatedTextSurface("default", 1, 1, font);
            playerAnimation.CreateFrame();
            playerAnimation.CurrentFrame[0].Foreground = Color.Orange;
            playerAnimation.CurrentFrame[0].GlyphIndex = '@';

            Player = new GameObject(font)
            {
                Animation = playerAnimation,
                Position = new Point(1, 1)
            };

            GenerateMap();
        }
        #endregion

        #region Mapping

        private static Dictionary<TerrainType, CellAppearance> _mapTerrainToAppearance = new Dictionary
            <TerrainType, CellAppearance>()
        {
            {TerrainType.Floor, new Floor()},
            {TerrainType.Door, new Floor()},
            {TerrainType.StairsDown, new Floor()},
            {TerrainType.StairsUp, new Floor()},
            {TerrainType.Corner, new Wall()},
            {TerrainType.HorizontalWall, new Wall()},
            {TerrainType.VerticalWall, new Wall()},
            {TerrainType.Wall, new Wall()},
        };
        private void GenerateMap()
        {
            _map = new Map(Width, Height);
            var excavator = new GridExcavator();
            excavator.Excavate(_map);
            // Create the local cache of map data
            // 
            _mapData = new CellAppearance[Width, Height];

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
                    _mapData[iCol, iRow] = _mapTerrainToAppearance[_map[iCol, iRow].Terrain];
                    _mapData[iCol, iRow].CopyAppearanceTo(this[iCol, iRow]);
                }
            }

            Player.Position = _map.RandomFloorLocation().ToPoint();

            // Center the veiw area
            TextSurface.RenderArea = new Rectangle(Player.Position.X - (TextSurface.RenderArea.Width / 2),
                                                    Player.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                    TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

            Player.RenderOffset = Position - TextSurface.RenderArea.Location;
        }
        #endregion

        #region Player handling
        public void MovePlayerBy(Point amount)
        {
            // Get the position the player will be at
            Point newPosition = Player.Position + amount;
            TerrainType terrain = _map[newPosition.X, newPosition.Y].Terrain;

            // Check to see if the position is within the map
            if (new Rectangle(0, 0, Width, Height).Contains(newPosition)
                && terrain == TerrainType.Floor || terrain == TerrainType.Door)
            {
                // Move the player
                Player.Position += amount;
                _map.Player.Location = Player.Position.ToMapCoordinates();

                // Scroll the view area to center the player on the screen
                TextSurface.RenderArea = new Rectangle(Player.Position.X - (TextSurface.RenderArea.Width / 2),
                                                        Player.Position.Y - (TextSurface.RenderArea.Height / 2),
                                                        TextSurface.RenderArea.Width, TextSurface.RenderArea.Height);

                // If he view area moved, we'll keep our entity in sync with it.
                Player.RenderOffset = this.Position - TextSurface.RenderArea.Location;
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
