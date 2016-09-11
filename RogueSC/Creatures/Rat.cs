using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.Items;
using CSRogue.Utilities;
using CSRogue.GameControl.Commands;
using CSRogue.Map_Generation;

namespace RogueSC.Creatures
{
    public class Rat : Creature
    {
        private Game _game;

        public Rat(Level l)
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
