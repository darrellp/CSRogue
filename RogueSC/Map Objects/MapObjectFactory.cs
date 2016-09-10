using System.Collections.Generic;
using CSRogue.Map_Generation;
using Microsoft.Xna.Framework;
using SadConsole;

namespace RogueSC.Map_Objects
{
    public static class MapObjectFactory
    {
        internal static Dictionary<string, CellAppearance> ObjectNameToAppearance = new Dictionary<string, CellAppearance>()
        {
            {"wall", new CellAppearance(Color.Gray, Color.Blue, 176)},
            {"floor", new CellAppearance(Color.Orange, Color.Transparent, 46)},
            {"Rat", new CellAppearance(Color.Red, Color.Transparent, 114)},
        };

        private static readonly CellAppearance wallAppearance = new CellAppearance(Color.Gray, Color.Blue, 176);
        private static readonly CellAppearance floorAppearance = new CellAppearance(Color.Orange, Color.Transparent, 46);

        /// <summary>   Maps terrain types to appearance for that terrain. </summary>
        /// Note that "Appearance" means a character, a foreground and a background color.  We display hidden and remembered
        /// cells by using the same appearance and changing the effect on it with a Recolor effect which modifies the foreground
        /// and background colors without changing the glyph.  So, in reality as things stand now with a static background we
        /// just need to change effects on newly seen and newly unseen characters.
        internal static readonly Dictionary<TerrainType, CellAppearance> MapTerrainToAppearance = new Dictionary
            <TerrainType, CellAppearance>()
        {
            {TerrainType.Floor, floorAppearance},
            {TerrainType.Door, floorAppearance},
            {TerrainType.StairsDown, floorAppearance},
            {TerrainType.StairsUp, floorAppearance},
            {TerrainType.Corner, wallAppearance},
            {TerrainType.HorizontalWall, wallAppearance},
            {TerrainType.VerticalWall, wallAppearance},
            {TerrainType.Wall, wallAppearance},
        };

        internal static MapObjectAppearance CreateObject(string type)
        {
            return new MapObjectAppearance(ObjectNameToAppearance[type]);
        }
    }
}