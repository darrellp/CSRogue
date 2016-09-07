using CSRogue.GameControl;
using CSRogue.Items;

namespace CSRogue.Map_Generation
{
    // An interface to use with the Game object
    public interface IGameMap : IRoomsMap
    {
        FOV Fov { get; set; }
        Game Game { get; }
        IPlayer Player { get; }
    }
}