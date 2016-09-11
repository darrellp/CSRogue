using CSRogue.GameControl;
using CSRogue.Items;
using CSRogue.Map_Generation;

namespace CSRogue.Interfaces
{
    // An interface to use with the Game object
    public interface IGameMap : IMap
    {
        FOV Fov { get; set; }
        Game Game { get; }
        IPlayer Player { get; set; }
    }
}