using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Creatures
{
    internal class ChasingCreature : SCCreature
    {
        public ChasingCreature(ILevel l, ItemInfo i) : base(l, i) { }

        public override void InvokeAi()
        {
            var neighbors = Game.Map.Neighbors(Location).ToList();
            var playerLocation = Game.Map.Player.Location;
            MapCoordinates dest = default(MapCoordinates);
            bool fAStarSucceeded = false;
            if (Game.Map[playerLocation].Room == Game.Map[Location].Room)
            {
                var astar = new AStar(Location, playerLocation, Game.Map);
                var path = astar.Solve(c => Game.Map.Walkable(c) && (!Game.Map.IsCreatureAt(c) || c == playerLocation));
                if (path != null)
                {
                    dest = path[1];
                    fAStarSucceeded = true;
                }
            }
            if (!fAStarSucceeded)
            {
                IList<MapCoordinates> select =
                    Selector<MapCoordinates>.SelectFrom(neighbors, loc => Game.Map[loc].Terrain == TerrainType.Floor);
                if (select.Count == 0)
                {
                    return;
                }
                dest = select[0];
            }
            Game.Enqueue(new MoveToCommand(this, dest));
        }
    }
}
