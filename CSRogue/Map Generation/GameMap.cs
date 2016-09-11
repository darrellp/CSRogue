using System;
using CSRogue.GameControl;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
    public class GameMap : BaseMap, IGameMap
    {
        private FOV _fov;
        private readonly Game _game;
        private IPlayer _player;

        public FOV Fov
        {
            get { return _fov; }

            set { _fov = value; }
        }

        public Game Game
        {
            get { return _game; }
        }

        public IPlayer Player
        {
            get { return _player; }

            set { _player = value; }
        }

        public GameMap(int fovRadius, Game game, string mapString, Func<IMapLocationData> dataCreator = null) : base(mapString, game. Factory, dataCreator)
        {
            _fov = new FOV(this, fovRadius);
            _game = game;
            this.SetPlayer(true);
        }

        public GameMap(int height, int width, int fovRadius, Game game, IPlayer player, Func<IMapLocationData> dataCreator = null) : base(height, width, dataCreator)
        {
            _fov = new FOV(this, fovRadius);
            _game = game;
	        _player = player;
        }
    }
}
