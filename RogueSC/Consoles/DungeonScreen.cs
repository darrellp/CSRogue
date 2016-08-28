using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Consoles;
using SadConsole.Input;
using Console = SadConsole.Consoles.Console;
using Input = Microsoft.Xna.Framework.Input;

namespace RogueSC.Consoles
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A dungeon screen. </summary>
    ///
    /// <remarks>   This is the console that holds and coordinates all the other consoles.
    ///             Darrellp, 8/26/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class DungeonScreen : ConsoleList
    {
        #region Public Properties
        /// <summary>   The dungeon console. </summary>
        public DungeonMapConsole DungeonConsole;
        /// <summary>   The statistics console. </summary>
        public CharacterConsole StatsConsole;
        /// <summary>   The message console. </summary>
        public MessagesConsole MessageConsole;
        #endregion

        #region Private Variables
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Console messageHeaderConsole;
        #endregion

        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DungeonScreen()
        {
            StatsConsole = new CharacterConsole(24, 17);
            // ReSharper disable once RedundantArgumentDefaultValue
            DungeonConsole = new DungeonMapConsole(56, 16, 100, 100, Font.FontSizes.One);
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

            // Move the rest of the consoles into position (DungeonConsole is already in position at 0,0)
            StatsConsole.Position = new Point(56, 0);
            MessageConsole.Position = new Point(0, 18);
            messageHeaderConsole.Position = new Point(0, 17);

            // Add all consoles to this console list.
            Add(messageHeaderConsole);
            Add(StatsConsole);
            Add(DungeonConsole);
            Add(MessageConsole);

            // Placeholder stuff for the stats screen
            StatsConsole.CharacterName = "Hydorn";
            StatsConsole.MaxHealth = 200;
            StatsConsole.Health = 100;

            Engine.ActiveConsole = this;
            Engine.Keyboard.RepeatDelay = 0.1f;
            Engine.Keyboard.InitialRepeatDelay = 0.1f;
        }
        #endregion

        #region Keyboard handling
        /// <summary>   A dictionary to map the arrow keys to their corresponding movement. </summary>
        private static readonly Dictionary<Input.Keys, Point> KeysToMovement = new Dictionary<Input.Keys, Point>()
            {
                {Input.Keys.Up, new Point(0, -1) },
                {Input.Keys.Down, new Point(0, 1) },
                {Input.Keys.Left, new Point(-1, 0) },
                {Input.Keys.Right, new Point(1, 0) },
                {Input.Keys.End, new Point(-1, 1) },
                {Input.Keys.PageUp, new Point(1, -1) },
                {Input.Keys.PageDown, new Point(1, 1) },
                {Input.Keys.Home, new Point(-1, -1) },
            };

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            if (info.KeysPressed.Count == 0)
            {
                return false;
            }

            if (KeysToMovement.ContainsKey(info.KeysPressed[0].XnaKey))
            {
                DungeonConsole.MovePlayerBy(KeysToMovement[info.KeysPressed[0].XnaKey]);
            }

            return false;
        }
        #endregion
    }
}
