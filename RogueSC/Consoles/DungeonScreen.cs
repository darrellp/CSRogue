using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;

namespace RogueSC.Consoles
{
    class DungeonScreen : ConsoleList
    {
        public Console ViewConsole;
        public CharacterConsole StatsConsole;
        public MessagesConsole MessageConsole;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Console messageHeaderConsole;

        public DungeonScreen()
        {
            StatsConsole = new CharacterConsole(24, 17);
            ViewConsole = new Console(56, 17);
            ViewConsole.FillWithRandomGarbage(); // Temporary so we can see where the console is on the screen
            MessageConsole = new MessagesConsole(80, 6);

            // Setup the message header to be as wide as the screen but only 1 character high
            messageHeaderConsole = new Console(80, 1)
            {
                DoUpdate = false,
                CanUseKeyboard = false,
                CanUseMouse = false
            };

            // Draw the line for the header
            messageHeaderConsole.Fill(Color.White, Color.Black, 196, null);
            messageHeaderConsole.SetGlyph(56, 0, 193); // This makes the border match the character console's left-edge border

            // Print the header text
            messageHeaderConsole.Print(2, 0, " Messages ");

            // Move the rest of the consoles into position (ViewConsole is already in position at 0,0)
            StatsConsole.Position = new Point(56, 0);
            MessageConsole.Position = new Point(0, 18);
            messageHeaderConsole.Position = new Point(0, 17);

            // Add all consoles to this console list.
            Add(messageHeaderConsole);
            Add(StatsConsole);
            Add(ViewConsole);
            Add(MessageConsole);

            // Placeholder stuff for the stats screen
            StatsConsole.CharacterName = "Hydorn";
            StatsConsole.MaxHealth = 200;
            StatsConsole.Health = 100;
        }
    }
}
