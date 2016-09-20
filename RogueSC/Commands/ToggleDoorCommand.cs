using System;
using CSRogue.GameControl.Commands;
using CSRogue.Interfaces;
using CSRogue.Map_Generation;
using Game = CSRogue.GameControl.Game;

namespace RogueSC.Commands
{
    class ToggleDoorCommand : IRogueCommand
    {
        private readonly ICreature _agent;
        private readonly MapCoordinates _doorLocation;

        /// <summary> Event queue for all listeners interested in new level events. </summary>
        internal static event EventHandler<ToogleDoorEventArgs> ToggleDoorEvent;
        internal void InvokeToggleDoorEvent(Object sender, ToogleDoorEventArgs e)
        {
            ToggleDoorEvent?.Invoke(sender, e);
        }


        internal ToggleDoorCommand(ICreature agent, MapCoordinates doorLocation)
        {
            _doorLocation = doorLocation;
            _agent = agent;
        }

        public void Execute(Game game)
        {
            SCMap scMap = (SCMap) game.Map;
            scMap[_doorLocation].ToggleDoor();
            InvokeToggleDoorEvent(game, new ToogleDoorEventArgs(_agent, _doorLocation));
            var moveCmd = new MovePlayerCommand(new MapCoordinates());
            game.EnqueueAndProcess(moveCmd);
        }
    }
}
