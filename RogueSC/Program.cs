﻿using System;
using SadConsole;

namespace RogueSC
{
    class Program
    {
        static void Main()
        {
            // Setup the engine and creat the main window.
            Engine.Initialize("IBM.font", 80, 25);

            // Hook the start event so we can add consoles to the system.
            Engine.EngineStart += Engine_EngineStart;

            // Hook the update event that happens each frame so we can trap keys and respond.
            Engine.EngineUpdated += Engine_EngineUpdated;

            // Start the game .
            Engine.Run();
        }

        private static void Engine_EngineStart(object sender, EventArgs e)
        {
            // Clear the default console
            Engine.ConsoleRenderStack.Clear();
            Engine.ActiveConsole = null;

            GameWorld.Start();
        }

        private static void Engine_EngineUpdated(object sender, EventArgs e)
        {

        }
    }
}
