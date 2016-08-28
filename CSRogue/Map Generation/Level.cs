using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Item_Handling;
using CSRogue.Items;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	public class Level
	{
		#region Private fields
		private readonly Game _game;
		private readonly List<Creature> _creatures = new List<Creature>();
		#endregion

		#region Public Properties
		public int Depth { get; }
		public Map Map { get; }
		#endregion

		#region Constructor
		public Level(int depth, int width, int height, Game game = null, IExcavator excavator = null, int seed = -1)
		{
			_game = game;
			Map = new Map(width, height, _game);
			Depth = depth;
			if (excavator == null)
			{
				excavator = new GridExcavator(seed);
			}
			excavator.Excavate(Map);
			DistributeItems();
		}

		protected virtual int CreatureCount()
		{
			return Map.Width * Map.Height / 1000 + Rnd.Global.Next(7) - 3;
		}

		private void DistributeItems()
		{
			DistributeCreatures();
			DistributeInanimate();
		}

		protected virtual void DistributeInanimate()
		{
		}

		protected virtual void DistributeCreatures()
		{
			// Get the creatures allowable on this level
			List<ItemInfo> creatureInfoList = ItemInfo.CreatureListForLevel(Depth);

			// Get the sum of their rarities
			int sumOfRarities = creatureInfoList.Sum(creatureItemInfo => creatureItemInfo.CreatureInfo.Rarity);

			// Are there no monsters allowed on this level?
			if (sumOfRarities == 0)
			{
				return;
			}

			// Figure out how many monsters on this level
			int creatureCount = CreatureCount();

			// For each creature
			for (int iCreature = 0; iCreature < creatureCount; iCreature++)
			{
				// Select a creature
				Creature creature = SelectCreature(creatureInfoList, sumOfRarities);

				// Place the selected creature
				PlaceCreature(creature);
			}
		}

		private static Creature SelectCreature(IEnumerable<ItemInfo> creatureList, int sumOfRarities)
		{
			int cumulationLimit = Rnd.Global.Next(sumOfRarities);
			int rarityCumulation = 0;

			foreach (ItemInfo creature in creatureList)
			{
				rarityCumulation += creature.CreatureInfo.Rarity;
				if (rarityCumulation > cumulationLimit)
				{
					return (Creature)ItemInfo.NewItemFromChar(creature.Character);
				}
			}
			throw new RogueException("Couldn't find creature in SelectCreature");
		}

		private void PlaceCreature(Creature creature)
		{
			// Find a random floor location
			MapCoordinates location = Map.RandomFloorLocation();

			// Is there a creature there?
			while (Map[location].Items.Any(item => ItemInfo.GetItemInfo(item.ItemType).IsCreature))
			{
				// Find another position
				location = Map.RandomFloorLocation();
			}

			// Place the monster on the map
			Map.Drop(location, creature);
			creature.Location = location;
			_creatures.Add(creature);
		}
		#endregion

		public void KillCreature(Creature victim)
		{
			Map.Remove(victim.Location.Column, victim.Location.Row, victim);
			_creatures.Remove(victim);
		}

		public void InvokeMonsterAI()
		{
			foreach (var creature in _creatures)
			{
				if (!creature.IsPlayer)
				{
					var neighbors = Map.Neighbors(creature.Location).ToList();
					IList<MapCoordinates> select =
						Selector<MapCoordinates>.SelectFrom(neighbors, loc => Map[loc].Terrain == TerrainType.Floor);
					if (select.Count == 0)
					{
						break;
					}
					_game.Enqueue(new MoveToCommand(creature, select[0]));
				}
			}
		}
	}
}
