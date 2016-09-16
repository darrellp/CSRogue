using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Effects;
using System.Collections.Generic;
using CSRogue.Map_Generation;

namespace RogueSC.Map_Objects
{
    internal static class ScRender
    {
        public const int WallGlyph = 177;
        public const int DoorGlyph = 176;
        public const int FloorGlyph = 46;
        public const int PlayerGlyph = 111;

        static readonly ICellEffect EffectSeen = new Recolor()
        {
            Foreground = Color.LightGray * 0.6f,
            Background = Color.LightGray * 0.2f,
            DoForeground = true,
            DoBackground = true,
            CloneOnApply = false
        };

        static readonly ICellEffect EffectHidden = new Recolor()
        {
            Foreground = Color.Black,
            Background = Color.Black,
            DoForeground = true,
            DoBackground = true,
            CloneOnApply = false
            };

        internal static Dictionary<string, CellAppearance> ObjectNameToAppearance = new Dictionary<string, CellAppearance>()
        {
            {"wall", new CellAppearance(Color.Gray, Color.Blue, WallGlyph)},
            {"floor", new CellAppearance(Color.Orange, Color.Transparent, FloorGlyph) },
            {"Rat", new CellAppearance(Color.Red, Color.Transparent, 'r') },
            {"Orc", new CellAppearance(Color.Yellow, Color.Transparent, 'o') },
			{"Player", new CellAppearance(Color.Yellow, Color.Transparent, PlayerGlyph) /* really a noop */ },
			{"openDoor", new CellAppearance(Color.Yellow, Color.Transparent, DoorGlyph) },
			{"closedDoor", new CellAppearance(Color.Orange, Color.Transparent, DoorGlyph) },
		};

        internal static readonly CellAppearance WallAppearance = new CellAppearance(Color.Gray, Color.Blue, WallGlyph);
        internal static readonly CellAppearance FloorAppearance = new CellAppearance(Color.Orange, Color.Transparent, FloorGlyph);
		internal static readonly CellAppearance OffMapAppearance = new CellAppearance(Color.Black, Color.Black, FloorGlyph);
		internal static readonly CellAppearance DoorAppearance = new CellAppearance(Color.Orange, Color.Transparent, DoorGlyph);

		/// <summary>   Maps terrain types to appearance for that terrain. </summary>
		/// Note that "Appearance" means a character, a foreground and a background color.  We display hidden and remembered
		/// cells by using the same appearance and changing the effect on it with a Recolor effect which modifies the foreground
		/// and background colors without changing the glyph.  So, in reality as things stand now with a static background we
		/// just need to change effects on newly seen and newly unseen characters.
		internal static readonly Dictionary<TerrainType, CellAppearance> MapTerrainToAppearance = new Dictionary
		    <TerrainType, CellAppearance>()
	    {
		    {TerrainType.Floor, FloorAppearance},
		    {TerrainType.Door, DoorAppearance},
		    {TerrainType.StairsDown, FloorAppearance},
		    {TerrainType.StairsUp, FloorAppearance},
		    {TerrainType.Corner, WallAppearance},
		    {TerrainType.HorizontalWall, WallAppearance},
		    {TerrainType.VerticalWall, WallAppearance},
		    {TerrainType.Wall, WallAppearance},
		    {TerrainType.OffMap, OffMapAppearance }
	    };

	    public static void RenderToCell(CellAppearance appearance, Cell sadConsoleCell, bool isFov)
        {
            appearance.CopyAppearanceTo(sadConsoleCell);

            // Clear out the old effect if there was one
            if (sadConsoleCell.Effect != null)
            {
                sadConsoleCell.Effect.Clear(sadConsoleCell);
                sadConsoleCell.Effect = null;
            }

            if (isFov)
            {
                // Do nothing if it's in view, it's a normal colored square
                // You could do something later like check how far the cell is from the player and tint it
            }
            else
            {
                sadConsoleCell.Effect = EffectSeen;
                sadConsoleCell.Effect.Apply(sadConsoleCell);
            }
        }

        public static void RemoveCellFromView(Cell sadConsoleCell)
        {
            // Clear out the old effect if there was one
            if (sadConsoleCell.Effect != null)
            {
                sadConsoleCell.Effect.Clear(sadConsoleCell);
                sadConsoleCell.Effect = null;
            }

            sadConsoleCell.Effect = EffectHidden;
            sadConsoleCell.Effect.Apply(sadConsoleCell);
        }
    }
}


