using CSRogue.Interfaces;

namespace CSRogue.RogueEventArgs
{
	public class NewLevelEventArgs : System.EventArgs
	{
		public ILevel PrevLevel { get; set; }
		public ILevel NewLevel { get; set; }
	}
}
