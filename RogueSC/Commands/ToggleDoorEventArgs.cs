using CSRogue.Interfaces;
using CSRogue.Map_Generation;

namespace RogueSC.Commands
{
    internal class ToogleDoorEventArgs : System.EventArgs
    {
        internal ToogleDoorEventArgs(ICreature agent, MapCoordinates doorLocation)
        {
            Agent = agent;
            DoorLocation = doorLocation;
        }

        internal ICreature Agent { get; private set; }
        internal MapCoordinates DoorLocation { get; private set; }
    }
}
