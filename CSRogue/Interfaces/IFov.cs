using System.Collections.Generic;
using CSRogue.Map_Generation;

namespace CSRogue.Interfaces
{
    public interface IFov
    {
        // Locations we saw on the previous scan
        HashSet<MapCoordinates> OldFov { get; }

        // Locations which are visible as of the last scan
        HashSet<MapCoordinates> CurrentFov { get; }

        /// <summary>   Swaps the old and current hash sets. </summary>
        void SwapHashSets();

        // SingleScan should do nothing more than put the currently seen values in CurrentFov
        void SingleScan(MapCoordinates crd);
    }
}