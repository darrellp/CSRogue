using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Creatures
{
    internal class ChasingCreature : Creature
    {
        protected Game _game;

        public ChasingCreature(Level l)
        {
            _game = l.Map.Game;
        }

        public override void InvokeAi()
        {
            var neighbors = _game.Map.Neighbors(Location).ToList();
            var playerLocation = _game.Map.Player.Location;
            var dest = Location;
            if (_game.Map[playerLocation].Room == _game.Map[Location].Room)
            {
                var min = int.MaxValue;
                foreach (var neighbor in neighbors.Where(l => _game.Map[l].Terrain == TerrainType.Floor))
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
                    Selector<MapCoordinates>.SelectFrom(neighbors, loc => _game.Map[loc].Terrain == TerrainType.Floor);
                if (select.Count == 0)
                {
                    return;
                }
                dest = select[0];
            }
            _game.Enqueue(new MoveToCommand(this, dest));
        }
    }
}
