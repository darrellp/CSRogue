using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Creatures
{
    internal class ChasingCreature : Creature
    {
        protected Game Game;

        public ChasingCreature(ILevel l)
        {
            Game = l.Map.Game;
        }

        public override void InvokeAi()
        {
            var neighbors = Game.Map.Neighbors(Location).ToList();
            var playerLocation = Game.Map.Player.Location;
            var dest = Location;
            if (Game.Map[playerLocation].Room == Game.Map[Location].Room)
            {
                var min = int.MaxValue;
                foreach (var neighbor in neighbors.Where(l => Game.Map[l].Terrain == TerrainType.Floor))
                {
                    var delta = neighbor - playerLocation;
                    var metric = Math.Abs(delta.Column) + Math.Abs(delta.Row);
                    if (metric < min)
                    {
                        dest = neighbor;
                        min = metric;
                    }
                }
            }
            else
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
