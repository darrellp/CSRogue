namespace CSRogue.GameControl.Commands
{
	public class CommandBase : IRogueCommand
	{
		public CommandType Command { get; }

		public CommandBase(CommandType command)
		{
			Command = command;
		}
	}
}
