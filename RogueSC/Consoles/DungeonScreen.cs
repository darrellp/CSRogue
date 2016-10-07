#define FIXEDMAP
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CSRogue.GameControl.Commands;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.RogueEventArgs;
using CSRogue.Utilities;
using Microsoft.Xna.Framework;
using RogueSC.Creatures;
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
    /// <summary>   The dungeon screen which gathers together the various consoles.. </summary>
    ///
    /// <remarks>   This is the screen that holds and coordinates all the other consoles.  In SadConsole
    ///             a ConsoleList doesn't actually have a "Console" itself but is just a collection of
    ///             other consoles.  Those other consoles have sizes and positions but the ConsoleList
    ///             doesn't - it's just the collection of those other consoles.
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
        public static DungeonScreen BaseScreen;
        #endregion

        #region Private Variables
        // The factory is essentially the dictionary for everything in the game - all monsters, all items, etc.
        // It allows us to not only find out about objects but also to create them.
        private static readonly ItemFactory Factory;

        // Probably should be in a text file but for this little toy rogue, no big deal.
        public static string[] TolkienNames =
        {
            "alfwine", "abattarik", "adanedhel", "adanel", "adrahil", "adunakhor", "aegnor", "aerin", "agarwaen",
            "aikanaro", "aiwendil", "alatar", "alatariel", "alcarin", "aldamir", "aldarion", "aldaron", "aldor",
            "amandil", "amdir", "amlaith", "amras", "amrod", "amroth", "amrothos", "anaire", "anardil", "anarion",
            "anborn", "ancalagon", "ancalime", "ancalimon", "andrast", "anducal", "anfauglir", "andreth", "androg",
            "angbor", "angrod", "annatar", "arador", "araglas", "aragorn", "aragost", "arahad", "arahael", "aranarth",
            "arantar", "aranuir", "araphant", "araphor", "arassuil", "aratan", "aratar", "arathorn", "araval", "aravir",
            "aravorn", "aredhel", "argeleb", "argon", "argonui", "arien", "aros", "arthedain", "arvedui", "arvegil",
            "arveleg", "arwen", "asfaloth", "atanamir", "atanatar", "aule", "ausir", "avranc", "azaghal", "azog",
            "baldor", "balin", "baragund", "barahir", "barahir", "baran", "bard", "bauglir", "belecthor", "beleg",
            "belegorn", "belegund", "belemir", "beor", "beorn", "bereg", "beregond", "beren", "bergil", "bert",
            "beruthiel", "bifur", "boldog", "berylla", "bofur", "bolg", "bolger", "bombadil", "bombur", "bor", "borin",
            "boromir", "boron", "borondir", "brand", "brandir", "gormadoc", "meriadoc", "primula", "brego", "bregolas",
            "bregor", "brodda", "brytta", "bucca", "barliman", "calembel", "calimehtar", "calion", "calmacil",
            "calmacil", "caranthir", "carcharoth", "castamir", "cemendur", "celeborn", "celebrian", "celebrimbor",
            "celebrindor", "celegorm", "celepharn", "ceorl", "cirdan", "cirion", "ciryaher", "ciryandil", "ciryatan",
            "ciryon", "cotton", "curufin", "curunir", "daeron", "dáin", "deagol", "denethor", "deor", "deorwine",
            "dernhelm", "dior", "dis", "dori", "dorlas", "draugluin", "duilin", "durin", "dwalin", "earendil",
            "earendur", "amandil", "earnil", "earnur", "earwen", "ecthelion", "egalmoth", "eilinel", "elanor",
            "elbereth", "eldacar", "eldarion", "elemmakil", "elendil", "elendor", "elendur", "elenna", "elenwe",
            "elessar", "elfhelm", "elfhild", "elfwine", "elladan", "elmo", "elrohir", "elrond", "elros", "elu", "elwe",
            "elwing", "elven", "king", "emeldir", "emerie", "enel", "enelye", "eöl", "eomer", "eomund", "eönwe", "eorl",
            "eothain", "eotheod", "eowyn", "eradan", "erendis", "erestor", "erkenbrand", "iluvatar", "estel", "estelmo",
            "este", "falassion", "faniel", "faramir", "fastred", "feanor", "felarof", "fengel", "fili", "finarfin",
            "findis", "finduilas", "finduilas", "fingolfin", "fingon", "finrod", "finvain", "finwe", "firiel",
            "folcwine", "frea", "frealáf", "freawine", "freca", "frerin", "fror", "fuinur", "fundin", "galador",
            "galadriel", "galdor", "gamil", "gamling", "gandalf", "ghânburi", "gilgalad", "gildor", "gilrain",
            "gimilkhâd", "gimilzôr", "gimli", "ginglith", "girion", "glanhir", "glaurung", "gloin", "gloredhel",
            "glorfindel", "goldberry", "goldwine", "golfimbul", "gollum", "gorbag", "gorlim", "gorthaur", "gothmog",
            "gram", "grima", "grimbold", "grishnákh", "gror", "gwaihir", "gwathir", "gwindor", "hador", "halbarad",
            "haldad", "haldan", "haldar", "haldir", "haleth", "hallas", "halmir", "háma", "handir", "hardang", "hareth",
            "helm", "herion", "herucalmo", "herumor", "herunumen", "hirgon", "hiril", "hostamir", "huan", "hundar",
            "huor", "hurin", "hyarmendacil", "ibûn", "idril", "ilmare", "iluvatar", "imbar", "imin", "iminye", "imrahil",
            "indis", "inglor", "ingwe", "inziladûn", "inzilbêth", "irilde", "irime", "irmo", "isildur", "isilme",
            "isilmo", "ivriniel", "khamûl", "khîm", "kili", "arthedain", "lagduf", "lalaith", "legolas", "lenwe", "leod",
            "lindir", "lugdush", "luthien", "lurtz", "mablung", "maedhros", "maeglin", "maglor", "magor", "mahtan",
            "maiar", "malach", "mallor", "malvegil", "manthor", "manwe", "marach", "voronwe", "mauhur", "melian",
            "meleth", "meneldil", "meneldur", "mîm", "minalcar", "minardil", "minastir", "minyatur", "mirielar",
            "zimraphel", "mirielserinde", "mithrandir", "morgoth", "morwen", "morwen", "muzgash", "nahar", "náin",
            "námo", "narmacil", "narvi", "nerdanel", "nessa", "nienna", "nienor", "nimloth", "nimrodel", "niniel", "nom",
            "nori", "ohtar", "oin", "olorin", "olwe", "ondoher", "ori", "ornendil", "orodreth", "orome", "oropher",
            "orophin", "osse", "ostoher", "pallando", "palantir", "pelendur", "pengolodh", "pharazôn", "beruthiel",
            "radagast", "rian", "romendacil", "rumil", "lobelia", "lotho", "sador", "saeros", "sakalthôr", "salgant",
            "salmar", "saruman", "sauron", "scatha", "shadowfax", "shagrat", "shelob", "silmarien", "singollo",
            "siriondil", "smaug", "smeagol", "snowmane", "soronto", "strider", "surion", "elmar", "tarcil", "tarondor",
            "tarannon", "tata", "tatie", "telchar", "telemmaite", "telemnar", "telperien", "telumehtar", "thengel",
            "theoden", "theodred", "theodwyn", "thingol", "thorin", "thorondir", "thorondor", "thráin", "thranduil",
            "thror", "tilion", "tindomiel", "tinuviel", "adalgrim", "belladonna", "ferumbras", "fortinbras", "gerontius",
            "isumbras", "paladin", "peregrin", "pervinca", "tulkas", "tuor", "turgon", "turambar", "turin", "ufthak",
            "ugluk", "uinen", "uldor", "ulfang", "ulfast", "ulwarth", "ulmo", "umbardacil", "undomiel", "ungoliant",
            "uole", "kuvion", "urwen", "vaire", "valacar", "valandil", "valandur", "vána", "vanimelde", "varda",
            "vardamir", "nolimon", "vidugavia", "vidumavi", "vinyarion", "vorondil", "voronwe", "walda", "wormtongue",
            "yavanna", "yávien", "zimraphel", "zimrathôn"
        };

        private readonly CSRogue.GameControl.Game _game;
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
        private int _levelDepth;
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
            StatsConsole = new CharacterConsole(StatsWidth, StatsHeight);
            MessageConsole = new MessagesConsole(WindowWidth, MessageHeight);

            BaseScreen = this;
#if FIXEDMAP
			var seed = 0;
#else
			var seed = -1;
#endif
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
            CreateNewLevel();

            DungeonConsole = new DungeonMapConsole(_game, DungeonWidth, DungeonHeight);

            // Setup the message header to be as wide as the screen but only 1 character high
            var messageHeaderConsole = new Console(WindowWidth, 1)
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
            StatsConsole.CharacterName = (new NameGenerator(TolkienNames, 3, 0.01)).Generate(5, 9);

            Engine.ActiveConsole = this;
            Engine.Keyboard.RepeatDelay = 0.1f;
            Engine.Keyboard.InitialRepeatDelay = 0.3f;

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

        private void CreateNewLevel()
        {
            var map = new SCMap(MapWidth, MapHeight, 10, _game, _game.Map?.Player, Factory);

            var levelCmd = new NewLevelCommand(_levelDepth++, map, new Dictionary<Guid, int>()
            {
                {ItemIDs.RatId, 1},
                {ItemIDs.OrcId, 1},
                {ItemIDs.SwordId, 1},
            });
            _game.EnqueueAndProcess(levelCmd);

            DungeonConsole?.NewLevelInit();
        }

        private void NewLevelPlayerCheck()
        {
            var map = (SCMap) _game.Map;
            if (map[map.Player.Location].Terrain == TerrainType.StairsDown)
            {
                CreateNewLevel();
            }
        }
#endregion

#region Messaging
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Print a message. </summary>
        ///
        /// <remarks>   Darrell, 9/18/2016. </remarks>
        ///
        /// <param name="msg">  The message to print. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal static void PrintLine(string msg)
        {
            BaseScreen.MessageConsole.PrintMessage(msg);
        }
#endregion

#region Event Handlers
        private void Game_AttackEvent(object sender, AttackEventArgs e)
        {
            bool involvesPlayer = e.Victim.IsPlayer || e.Attacker.IsPlayer;
            // Determine damage
            var damage = ((SCCreature) e.Attacker).CalculateDamage((SCCreature)e.Victim);

            if (involvesPlayer)
            {
                string msg = e.Attacker.IsPlayer
                    ? $"[c:r f:Red]You hit the [c:r f:Yellow]{Factory.InfoFromId[e.Victim.ItemTypeId].Name}[c:r f:Red] for [c:r f:Blue]{damage}[c:r f:Red] points"
                    : $"[c:r f:Red]The [c:r f:Yellow]{Factory.InfoFromId[e.Attacker.ItemTypeId].Name}[c:r f:Red] hit you for [c:r f:Blue]{damage}[c:r f:Red] points";

                PrintLine(msg);
            }

            // Hit the victim for that damage
            ((SCCreature)e.Victim).HitPoints -= damage;
            var expired = ((SCCreature)e.Victim).HitPoints <= 0;

            // Did the victim die?
            
			// Currently the player never dies.  Hooray for the player!
            if (expired && !e.Victim.IsPlayer)
            {
                // Kill the unfortunate victim
                // TODO: check to see if the victim is the player in which case, game over!
                _game.CurrentLevel.KillCreature(e.Victim);
            }

            if (e.Victim.IsPlayer || !expired)
            {
                return;
            }
            if (involvesPlayer)
            {
                PrintLine($"[c:r f:Red]You killed a mighty [c:r f:Yellow]{_game.Factory.InfoFromId[e.Victim.ItemTypeId].Name}!");
            }
        }

		private void Game_NewLevelEvent(object sender, NewLevelEventArgs e)
		{
		    if (e.NewLevel.Depth != 0)
		    {
		        if (((Hero) e.NewLevel.Map.Player).Inventory.Count > 0)
		        {
                    PrintLine("[c:r f:Blue]Your sword is ripped from your hands as you feel yourself disassociating...");
		            ((Hero) e.NewLevel.Map.Player).Inventory.Clear();
		        }
                else
		        {
                    PrintLine("[c:r f:Blue]you feel yourself disassociating...");
		        }
                PrintLine("[c:r f:Blue]Your atoms begin to fly apart as you feel yourself sucked down into...");
                PrintLine($"[c:r f:Yellow]Level {e.NewLevel.Depth}");
            }
            var map = (SCMap)e.NewLevel.Map;
			for (var iCol = 0; iCol < map.Width; iCol++)
			{
				for (var iRow = 0; iRow < map.Height; iRow++)
				{
					var data = map[iCol, iRow];
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
                {Input.Keys.O, (s)=> s.DungeonConsole.ToggleDoors() },
                {Input.Keys.G, (s)=> s.DungeonConsole.GetItem() },
            };

        /// <summary>   A dictionary to map the arrow keys to their corresponding movement. </summary>
        private static readonly Dictionary<Input.Keys, Action<DungeonScreen>> ShiftKeysToAction = new Dictionary<Input.Keys, Action<DungeonScreen>>()
            {
                // Does this ALWAYS correspond to '>'?  Doubt it but I'm not sure how to recognize
                // '>' from a shifted period key.
                {Input.Keys.OemPeriod, (s) => s.NewLevelPlayerCheck() },
            };

        private static Input.Keys _lastKeyActedOn = 0;

	    public override bool ProcessKeyboard(KeyboardInfo info)
        {
            if (info.KeysPressed.Count == 0)
            {
                // If the user has taken their finger off the key then reset LastKeyActedOn so we can
                // press "O" several times but not by holding down on the key.  Keeps us from opening
                // a door and then accidentally closing it again while holding down the key.
                if (info.IsKeyUp(Input.Keys.O))
                {
                    _lastKeyActedOn = 0;
                }
                return false;
            }
            // Just have this so during debugging I can put a bp on the following statement and hit
            // shift-X and see the x rather than the shift.
            if (info.KeysPressed[0].XnaKey == Input.Keys.LeftShift)
            {
                return false;
            }
            bool fShift = Input.Keyboard.GetState().IsKeyDown(Input.Keys.LeftShift) ||
                          Input.Keyboard.GetState().IsKeyDown(Input.Keys.RightShift);


            if (_lastKeyActedOn == Input.Keys.O && info.KeysPressed[0].XnaKey == Input.Keys.O)
	        {
	            return false;
	        }
			if (!fShift && KeysToAction.ContainsKey(info.KeysPressed[0].XnaKey))
			{
			    _lastKeyActedOn = info.KeysPressed[0].XnaKey;
                _writer?.WriteLine(info.KeysPressed[0].XnaKey.ToString());
			    ActOnKey(info.KeysPressed[0].XnaKey);
			}
            else if (fShift && ShiftKeysToAction.ContainsKey(info.KeysPressed[0].XnaKey))
			{
                // TODO: the recorder won't understand this - make it see the light
                _writer?.WriteLine($"Shft-{info.KeysPressed[0].XnaKey}");
                ActOnKey(info.KeysPressed[0].XnaKey, true);
            }
            return false;
        }

        private void ActOnKey(Input.Keys key, bool fShift = false)
        {
            if (fShift)
            {
                ShiftKeysToAction[key](this);
            }
            else
            {
                KeysToAction[key](this);
            }
            DungeonConsole.CheckFOV();
        }
#endregion
    }
}
