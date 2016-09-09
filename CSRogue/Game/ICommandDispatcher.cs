using CSRogue.GameControl.Commands;

namespace CSRogue.GameControl
{
    public interface ICommandDispatcher
    {
        void Dispatch(IRogueCommand command);
    }
}