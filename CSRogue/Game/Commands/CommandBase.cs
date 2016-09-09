namespace CSRogue.GameControl.Commands
{
	public class CommandBase : IRogueCommand
	{
	    public virtual bool Execute(Game game)
	    {
	        return false;
	    }

	    public CommandBase()
	    {
	    }
	}
}
