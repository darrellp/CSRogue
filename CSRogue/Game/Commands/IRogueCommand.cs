using CSRogue.Map_Generation;

namespace CSRogue.GameControl.Commands
{
	public interface IRogueCommand
	{
	    bool Execute(Game game);
	}
}
