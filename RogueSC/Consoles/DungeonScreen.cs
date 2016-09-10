using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CSRogue.GameControl.Commands;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;
using Microsoft.Xna.Framework;
using RogueSC.Creatures;
using RogueSC.Utilities;
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
        private CSRogue.GameControl.Game _game;
        private static ItemFactory _factory;
        private const int MapWidth = 100;
        private const int MapHeight = 100;
        #endregion

        #region Constructor
        static DungeonScreen()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "RogueSC.TextFiles.ItemFactoryData.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var reader = new StreamReader(stream);
                _factory = new ItemFactory(reader);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DungeonScreen()
        {
            _game = new CSRogue.GameControl.Game(_factory);
            _game.AttackEvent += Game_AttackEvent;
            var player = (IPlayer) _factory.InfoFromId[ItemIDs.HeroId].CreateItem(null);
            var map = new GameMap(MapWidth, MapHeight, 10, _game, player);
            var excavator= new GridExcavator();
            excavator.Excavate(map, player);
            var levelCmd = new NewLevelCommand(0, map, new Dictionary<Guid, int>()
            {
                {ItemIDs.RatId, 1},
                {ItemIDs.OrcId, 1}
            });
            _game.EnqueueAndProcess(levelCmd);

            StatsConsole = new CharacterConsole(24, 17);
            // ReSharper disable once RedundantArgumentDefaultValue
            DungeonConsole = new DungeonMapConsole(_game, 56, 16);
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

        private void Game_AttackEvent(object sender, CSRogue.RogueEventArgs.AttackEventArgs e)
        {
            if (e.Victim.IsPlayer || !e.VictimDied)
            {
                return;
            }
            MessageConsole.PrintMessage($"[c:r f:Red]You killed a mighty [c:r f:Yellow]{_game.Factory.InfoFromId[e.Victim.ItemTypeId].Name}!");

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
