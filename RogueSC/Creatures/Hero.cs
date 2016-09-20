using CSRogue.Interfaces;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using RogueSC.Consoles;

namespace RogueSC.Creatures
{
    internal sealed class Hero : SCCreature, IPlayer
    {
        private int _maxHitPointsBase;
        private int _regain;
        private int _vitality = 30;

        public int MaxHitPointsBase
        {
            get { return _maxHitPointsBase; }
            set
            {
                _maxHitPointsBase = value;
                DungeonScreen.BaseScreen.StatsConsole.MaxHealth = value;
            }
        }

        public override int HitPoints
        {
            set
            {
                base.HitPoints = value;
                DungeonScreen.BaseScreen.StatsConsole.Health = value;
            }
        }

        public Hero(Level l, ItemInfo i) : base(l, i)
        {
            DungeonScreen.BaseScreen.StatsConsole.MaxHealth = MaxHitPointsBase = HitPoints;
        }

        internal void Moved()
        {
            if (++_regain >= _vitality && HitPoints < MaxHitPointsBase)
            {
                _regain = 0;
                HitPoints++;
            }
        }
    }
}
