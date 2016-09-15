using System.Collections.Generic;
using System.Linq;
using CSRogue.Interfaces;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
    public static class FovExtensions
    {
        public static IEnumerable<MapCoordinates> NewlySeen(this IFov fov)
        {
            return fov.CurrentFov.Where(loc => !fov.OldFov.Contains(loc));
        }

        public static IEnumerable<MapCoordinates> NewlyUnseen(this IFov fov)
        {
            return fov.OldFov.Where(loc => !fov.CurrentFov.Contains(loc));
        }

        public static IEnumerable<MapCoordinates> CurrentlySeen(this IFov fov)
        {
            return fov.CurrentFov;
        }

        public static void Scan(this IFov fov, MapCoordinates loc)
        {
            fov.SwapHashSets();
            fov.CurrentFov.Clear();
            fov.SingleScan(loc);
        }
    }
}
