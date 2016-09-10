using System;
using System.Collections.Generic;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace CSRogue.GameControl.Commands
{
	public class NewLevelCommand : CommandBase
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the requested level. </summary>
		///
		/// <value>	level being requested. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public int Depth { get; set; }
        public IGameMap Map { get; set; }
        public Dictionary<Guid, int> Rarity { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Darrellp, 10/8/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public NewLevelCommand()
		{
		}

        public NewLevelCommand(int depth, IGameMap map, Dictionary<Guid, int> rarity = null )
        {
            Depth = depth;
            Map = map;
            Rarity = rarity;
        }

        public override bool Execute(Game game)
	    {
            game.CurrentLevel = new Level(Depth, Map, game.Factory, Rarity);
            Map.SetPlayer(true);
            game.InvokeEvent(EventType.NewLevel, this);
            return false;
	    }
	}
}
