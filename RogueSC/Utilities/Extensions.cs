using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRogue.Map_Generation;
using Microsoft.Xna.Framework;

namespace RogueSC.Utilities
{
    internal static class Extensions
    {
        internal static Point ToPoint(this MapCoordinates coords)
        {
            return new Point(coords.Column, coords.Row);
        }

        internal static MapCoordinates ToMapCoordinates(this Point point)
        {
            return new MapCoordinates(point.X, point.Y);
        }
    }
}
