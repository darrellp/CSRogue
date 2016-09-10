using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Creatures
{
    public class Orc : Creature
    {
        private Game _game;

        public Orc(Level l)
        {
            _game = l.Map.Game;
        }

        public override void InvokeAi()
        {
            var neighbors = _game.Map.Neighbors(Location).ToList();
            IList<MapCoordinates> select =
                Selector<MapCoordinates>.SelectFrom(neighbors, loc => _game.Map[loc].Terrain == TerrainType.Floor);
            if (select.Count == 0)
            {
                return;
            }
            _game.Enqueue(new MoveToCommand(this, select[0]));
        }
    }
}
