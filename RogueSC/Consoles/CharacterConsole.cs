using System;
using Microsoft.Xna.Framework;
using SadConsole;
using Console = SadConsole.Consoles.Console;

namespace RogueSC.Consoles
{
    class CharacterConsole : Console
    {
        private string _characterName;
        private int _health;
        private int _maxHealth;

        public string CharacterName
        {
            set { _characterName = value; RedrawPanel(); }
        }

        public int Health
        {
            set { _health = value; RedrawPanel(); }
        }

        public int MaxHealth
        {
            set { _maxHealth = value; RedrawPanel(); }
        }

        public CharacterConsole(int width, int height): base(width, height)
        {
        
        // Draw the side bar
        var line = new SadConsole.Shapes.Line
            {
                EndingLocation = new Point(0, height - 1),
                CellAppearance = {GlyphIndex = 179},
                UseEndingCell = false,
                UseStartingCell = false
            };
            line.Draw(this);
        }

        private void RedrawPanel()
        {
            Print(2, 2, _characterName);

            // Create a colored string that looks like 52/500
            var healthStatus = _health.ToString().CreateColored(Color.LightGreen, Color.Black, null) +
                                                    "/".CreateColored(Color.White, Color.Black, null) +
                                                    _maxHealth.ToString().CreateColored(Color.DarkGreen, Color.Black, null);

            // Align the string to the right side of the console
            Print(Width - 2 - healthStatus.ToString().Length, 2, healthStatus);
        }
    }
}
