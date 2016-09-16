using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CSRogue.GameControl.Commands;
using CSRogue.Item_Handling;
using CSRogue.RogueEventArgs;
using CSRogue.Utilities;
using Microsoft.Xna.Framework;
using RogueSC.Map_Objects;
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
        private readonly CSRogue.GameControl.Game _game;
        private static readonly ItemFactory Factory;
        private const int MapWidth = 100;
        private const int MapHeight = 100;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        internal const int WindowWidth = 120;
        internal const int WindowHeight = 42;
        const int StatsWidth = 24;
        const int DungeonWidth = WindowWidth - StatsWidth;
        const int StatsHeight = WindowHeight - MessageHeight - 2;
        // one less to account for MessageHeader
        const int MessageHeight = 6;
        const int DungeonHeight = StatsHeight;


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
                Factory = new ItemFactory(reader);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Darrellp, 8/26/2016. </remarks>
        ///
        /// <param name="writer">       Recording writer. </param>
        /// <param name="reader">       Playback reader. </param>
        /// <param name="fileIndex">    FileIndex for playback or recording. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal DungeonScreen(StreamWriter writer, StreamReader reader, int fileIndex)
        {
            var seed = -1;
            if (reader != null)
            {
                _reader = reader;
                // ReSharper disable once AssignNullToNotNullAttribute
                seed = int.Parse(_reader.ReadLine());
            }
            else if (writer != null)
            {
                _writer = writer;
                Random rnd = new Random();
                seed = rnd.Next();
                _writer.WriteLine(seed);
            }
            Rnd.SetGlobalSeed(seed);
            _game = new CSRogue.GameControl.Game(Factory);
			_game.NewLevelEvent += Game_NewLevelEvent;
			_game.AttackEvent += Game_AttackEvent;
            var map = new SCMap(MapWidth, MapHeight, 10, _game, Factory);

            var levelCmd = new NewLevelCommand(0, map, new Dictionary<Guid, int>()
            {
                {ItemIDs.RatId, 1},
                {ItemIDs.OrcId, 1}
            });
            _game.EnqueueAndProcess(levelCmd);

            StatsConsole = new CharacterConsole(StatsWidth, StatsHeight);
            DungeonConsole = new DungeonMapConsole(_game, DungeonWidth, DungeonHeight);
            MessageConsole = new MessagesConsole(WindowWidth, MessageHeight);

            // Setup the message header to be as wide as the screen but only 1 character high
            messageHeaderConsole = new Console(WindowWidth, 1)
            {
                DoUpdate = false,
                CanUseKeyboard = false,
                CanUseMouse = false
            };

            // Draw the line for the header
            messageHeaderConsole.Fill(Color.White, Color.Black, 196, null);
            // This is the "upside down T" that meets with the left border of the character console
            messageHeaderConsole.SetGlyph(DungeonWidth, 0, 193);

            // Print the header text
            var tag = string.Empty;
            if (_reader != null)
            {
                tag = $" - Reading dbg{fileIndex:0000}.txt";
            }
            else if (_writer != null)
            {
                tag = $" - Writing dbg{fileIndex:0000}.txt";
            }
            messageHeaderConsole.Print(2, 0, " Messages" + tag);

            // Move the rest of the consoles into position (DungeonConsole is already in position at 0,0)
            StatsConsole.Position = new Point(DungeonWidth, 0);
            MessageConsole.Position = new Point(0, StatsHeight + 1);
            messageHeaderConsole.Position = new Point(0, StatsHeight);

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

            if (_reader != null)
            {
                while (_reader.Peek() >= 0)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var inKey = (Input.Keys)Enum.Parse(typeof(Input.Keys), _reader.ReadLine());
                    ActOnKey(inKey);
                }
            }
        }
		#endregion

		#region Event Handlers
		private void Game_AttackEvent(object sender, AttackEventArgs e)
		{
			if (e.Victim.IsPlayer || !e.VictimDied)
            {
                return;
            }
            MessageConsole.PrintMessage($"[c:r f:Red]You killed a mighty [c:r f:Yellow]{_game.Factory.InfoFromId[e.Victim.ItemTypeId].Name}!");
        }

		private void Game_NewLevelEvent(object sender, NewLevelEventArgs e)
		{
			var map = e.NewLevel.Map;
			for (var iCol = 0; iCol < map.Width; iCol++)
			{
				for (var iRow = 0; iRow < map.Height; iRow++)
				{
					var data = (SCMapLocationData)map[iCol, iRow];
					data.Appearance = ScRender.MapTerrainToAppearance[map[iCol, iRow].Terrain];
				}
			}
		}
		#endregion

		#region Keyboard handling
		/// <summary>   A dictionary to map the arrow keys to their corresponding movement. </summary>
		private static readonly Dictionary<Input.Keys, Action<DungeonScreen>> KeysToAction = new Dictionary<Input.Keys, Action<DungeonScreen>>()
			{
				{Input.Keys.Up, (s) => s.DungeonConsole.MovePlayerBy(new Point(0, -1)) },
				{Input.Keys.Down, (s) => s.DungeonConsole.MovePlayerBy(new Point(0, 1)) },
				{Input.Keys.Left, (s) => s.DungeonConsole.MovePlayerBy(new Point(-1, 0)) },
				{Input.Keys.Right, (s) => s.DungeonConsole.MovePlayerBy(new Point(1, 0)) },
				{Input.Keys.End, (s) => s.DungeonConsole.MovePlayerBy(new Point(-1, 1)) },
				{Input.Keys.PageUp, (s) => s.DungeonConsole.MovePlayerBy(new Point(1, -1)) },
				{Input.Keys.PageDown,(s) => s.DungeonConsole.MovePlayerBy(new Point(1, 1)) },
				{Input.Keys.Home, (s) => s.DungeonConsole.MovePlayerBy(new Point(-1, -1)) },
				{(Input.Keys)12, (s) => s.DungeonConsole.MovePlayerBy(new Point(0, 0)) },
				{Input.Keys.O, (s)=> s.DungeonConsole.ToggleDoors() }
			};

	    public override bool ProcessKeyboard(KeyboardInfo info)
        {
            if (info.KeysPressed.Count == 0)
            {
                return false;
            }

			if (KeysToAction.ContainsKey(info.KeysPressed[0].XnaKey))
			{
			    _writer?.WriteLine(info.KeysPressed[0].XnaKey.ToString());
			    ActOnKey(info.KeysPressed[0].XnaKey);
			}
			return false;
        }

        private void ActOnKey(Input.Keys key)
        {
            KeysToAction[key](this);
            DungeonConsole.CheckFOV();
        }
        #endregion
    }
}
