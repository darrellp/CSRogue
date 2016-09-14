using System;
using SadConsole;
using System.IO;
// ReSharper disable once RedundantUsingDirective
using System.Linq;

namespace RogueSC
{
    class Program
    {
#pragma warning disable 649
        private static StreamWriter _writer;
        // ReSharper disable once NotAccessedField.Local
        private static StreamReader _reader;
        private static int _index;
#pragma warning restore 649

        static void Main()
        {
#if RECORDING
            const int readIndex = -1;
            const string debugFolder = "DebugFiles";
            Directory.CreateDirectory(debugFolder);
            var files = Directory.GetFiles(debugFolder).ToList();
            var nextIndex = files.Count == 0 ? 0 :
                files.
                    // ReSharper disable once PossibleNullReferenceException
                    Select(fn => int.Parse(Path.GetFileName(fn).Substring(3, 4))).
                    Last() + 1;
            var writeFileName = $"DebugFiles\\dbg{nextIndex:d4}.txt";
#pragma warning disable 162
            // ReSharper disable UnreachableCode
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once ExpressionIsAlwaysNull
            using (var writer = readIndex < 0 ? new StreamWriter(File.OpenWrite(writeFileName)) : null)
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                using (var reader = readIndex >= 0 ? new StreamReader(File.OpenRead($"DebugFiles\\dbg{readIndex:d4}.txt")) : null)
                {
                // ReSharper restore UnreachableCode
#pragma warning restore 162
                    _writer = writer;
                    // ReSharper disable once ExpressionIsAlwaysNull
                    _reader = reader;
                    if (_reader != null)
                    {
                        _index = readIndex;
                    }
                    else if (_writer != null)
                    {
                        _index = nextIndex;
                    }
#endif
                    // Setup the engine and creat the main window.
                    Engine.Initialize("IBM.font", 80, 25);

                    // Hook the start event so we can add consoles to the system.
                    Engine.EngineStart += Engine_EngineStart;

                    // Hook the update event that happens each frame so we can trap keys and respond.
                    Engine.EngineUpdated += Engine_EngineUpdated;

                    // Start the game .
                    Engine.Run();
                }
#if RECORDING
            }
        }
#endif

        private static void Engine_EngineStart(object sender, EventArgs e)
        {
            // Clear the default console
            Engine.ConsoleRenderStack.Clear();
            Engine.ActiveConsole = null;

            GameWorld.Start(_writer, _reader, _index);
        }

        private static void Engine_EngineUpdated(object sender, EventArgs e)
        {

        }
    }
}
