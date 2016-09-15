using CSRogue.Interfaces;

namespace CSRogue.GameControl.Commands
{
	public abstract class CommandBase : IRogueCommand
	{
		public abstract void Execute(Game game);

	    public CommandBase()
	    {
	    }
	}
}
