namespace RogueSC
{
    class GameWorld
    {
        public static Consoles.DungeonScreen DungeonScreen;

        /// <summary>
        /// Called one time to initiate everything. Assumes SadConsole has been setup and is ready to go.
        /// </summary>
        public static void Start()
        {
            DungeonScreen = new Consoles.DungeonScreen();
            SadConsole.Engine.ConsoleRenderStack.Add(DungeonScreen);
            DungeonScreen.MessageConsole.PrintMessage("Welcome to THE GAME...");
        }
    }
}
