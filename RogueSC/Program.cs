//#define RECORDING

using System;
using System.Diagnostics.CodeAnalysis;
using SadConsole;
using System.IO;
// ReSharper disable once RedundantUsingDirective
using System.Linq;

namespace RogueSC
{
    // With all the RECORDING #IFs, we get all sorts of spurious error messages from Resharper and
    // warning messages from the compiler here so just turn it all off once
    [SuppressMessage("ReSharper", "All")]
    class Program
    {
#pragma warning disable CS0649
        private static StreamWriter _writer;
        private static StreamReader _reader;
        private static int _index;
#pragma warning restore CS0649

        static void Main()
        {
#if RECORDING
            // Assuming RECORDING is turned on...
            // If readIndex is a nonnegative value then we will read from the corresponding .dbg file.
            // If it's negative then we will write to the next available .dbg file.  This is clumsy
            // and inconvenient, but better than searching forever for a non-repro problem and I don't
            // want to stop and take time to get a nice UI going right now.  The thing is this has to
            // be done before the consoles are up and running so I'm not sure how best to handle that.
            // I suppose we could make two "phases" - one bringing up some sort of debug UI and then the
            // actual game.  This will have to do for the moment.
            const int readIndex = 0;

            const string debugFolder = "DebugFiles";
            Directory.CreateDirectory(debugFolder);
            var files = Directory.GetFiles(debugFolder).ToList();
            var nextIndex = files.Count == 0 ? 0 :
                files.
                Select(fn => int.Parse(Path.GetFileName(fn).Substring(3, 4))).
                Last() + 1;
            var writeFileName = $"DebugFiles\\dbg{nextIndex:d4}.txt";

            using (var writer = readIndex < 0 ? new StreamWriter(File.OpenWrite(writeFileName)) : null)
            {
                using (var reader = readIndex >= 0 ? new StreamReader(File.OpenRead($"DebugFiles\\dbg{readIndex:d4}.txt")) : null)
                {
                    _writer = writer;
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
                    // Setup the engine and create the main window.
                    Engine.Initialize("IBM.font", 80, 25);

                    // Hook the start event so we can add consoles to the system.
                    Engine.EngineStart += Engine_EngineStart;

                    // Hook the update event that happens each frame so we can trap keys and respond.
                    Engine.EngineUpdated += Engine_EngineUpdated;

                    // We want to make sure and end gracefully
                    try
                    {
                        // Start the game .
                        Engine.Run();
                    }
                    catch (Exception)
                    {
                    }
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
