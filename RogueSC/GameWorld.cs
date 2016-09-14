using System.IO;

namespace RogueSC
{
    class GameWorld
    {
        public static Consoles.DungeonScreen DungeonScreen;

        /// <summary>
        /// Called one time to initiate everything. Assumes SadConsole has been setup and is ready to go.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        public static void Start(StreamWriter writer, StreamReader reader, int index)
        {
            DungeonScreen = new Consoles.DungeonScreen(writer, reader, index);
            SadConsole.Engine.ConsoleRenderStack.Add(DungeonScreen);
            DungeonScreen.MessageConsole.PrintMessage("Welcome to THE GAME...");
        }
    }
}
