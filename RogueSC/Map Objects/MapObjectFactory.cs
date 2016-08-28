using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole;

namespace RogueSC.Map_Objects
{
    public static class MapObjectFactory
    {
        internal static Dictionary<string, CellAppearance> ObjectNameToAppearance = new Dictionary<string, CellAppearance>()
        {
            {"wall", new CellAppearance(Color.White, Color.Gray, 176)},
            {"floor", new CellAppearance(Color.DarkGray, Color.Transparent, 46)}
        };

        internal static MapObject CreateObject(string type)
        {
            return new MapObject(ObjectNameToAppearance[type]);
        }
    }
}